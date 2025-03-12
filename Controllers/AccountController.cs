using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using IT15_TripoleMedelTijol.Models;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography; 

using System;


namespace IT15_TripoleMedelTijol.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 HttpClient httpClient,
                                 ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid login attempt.";
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                TempData["ErrorMessage"] = "Invalid Email or Password.";
                return View(model);
            }

            // ✅ Check if user has HR or Admin role
            if (!await _userManager.IsInRoleAsync(user, "HR") && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Access Denied. Only HR personnel or Admins can log in.";
                return View(model);
            }

            // ✅ Check last OTP request timestamp (Prevent multiple OTP requests)
            var lastOtpTimeStr = HttpContext.Session.GetString("LastOtpTime");
            if (lastOtpTimeStr != null && DateTime.TryParse(lastOtpTimeStr, out var lastOtpTime))
            {
                if ((DateTime.UtcNow - lastOtpTime).TotalSeconds < 60) // Prevent OTP request within 60 sec
                {
                    TempData["ErrorMessage"] = "OTP already sent. Please wait before requesting a new one.";
                    return View(model);
                }
            }

            // ✅ Generate a secure OTP
            var otp = GenerateSecureOtp();

            // ✅ Send OTP via SMS
            var phoneNumber = user.PhoneNumber;
            var sendData = new
            {
                sender_id = "PhilSMS",
                recipient = phoneNumber,
                message = $"Your OTP for login is: {otp}"
            };

            var token = "1179|EkuFgMmv7JYBgb0qwf5ZD6kZWKrvuqFyGHGb9g04"; // Replace with your actual token
            var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://app.philsms.com/api/v3/sms/send")
            {
                Headers =
                {
                    { "Authorization", $"Bearer {token}" }
                },
                Content = content
            };

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                _logger.LogError("❌ Failed to send OTP. API Response: {ErrorDetails}", errorDetails);
                TempData["ErrorMessage"] = "Failed to send OTP. Please try again.";
                return View(model);
            }

            // ✅ Store OTP, email, and request time for verification
            HttpContext.Session.SetString("Otp", otp);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("LastOtpTime", DateTime.UtcNow.ToString()); // Save timestamp

            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            var storedOtp = HttpContext.Session.GetString("Otp");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(userEmail) || storedOtp != otp)
            {
                TempData["ErrorMessage"] = "Invalid OTP. Please try again.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return View();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            HttpContext.Session.Remove("Otp");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("LastOtpTime"); // Remove OTP request time

            TempData["SuccessMessage"] = "Login successful!";
            return RedirectToAction("AdminDashboard", "Home");
        }

        // ✅ Secure OTP Generator
        private string GenerateSecureOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[4]; // 4 bytes = 32-bit integer
                rng.GetBytes(randomBytes);

                int otp = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % 1000000; // Ensure non-negative 6-digit OTP
                return otp.ToString("D6");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "User session expired. Please login again." });
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Generate new OTP
            var newOtp = GenerateSecureOtp();
            HttpContext.Session.SetString("Otp", newOtp);

            // Send OTP via PhilSMS API
            var phoneNumber = user.PhoneNumber;
            var sendData = new
            {
                sender_id = "PhilSMS",
                recipient = phoneNumber,
                message = $"Your new OTP is: {newOtp}"
            };

            var token = "1179|EkuFgMmv7JYBgb0qwf5ZD6kZWKrvuqFyGHGb9g04"; // Replace with your actual token
            var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://app.philsms.com/api/v3/sms/send")
            {
                Headers =
        {
            { "Authorization", $"Bearer {token}" }
        },
                Content = content
            };

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Failed to send OTP. Please try again." });
            }

            return Json(new { success = true, message = "OTP resent successfully." });
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
