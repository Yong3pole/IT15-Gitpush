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

            if (user == null)
            {
                // Simulate delay and failure to prevent user enumeration
                await Task.Delay(500);
                TempData["ErrorMessage"] = "Invalid Email or Password.";
                return View(model);
            }

            // ✅ Check if user is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                TempData["ErrorMessage"] = "Your account is temporarily locked due to multiple failed login attempts. Please try again later.";
                return View(model);
            }

            // ✅ Check password
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _userManager.AccessFailedAsync(user); // Track failed attempt
                TempData["ErrorMessage"] = "Invalid Email or Password.";
                return View(model);
            }

            // ✅ Password correct: Reset failed count
            await _userManager.ResetAccessFailedCountAsync(user);

            // ✅ Check role
            if (!await _userManager.IsInRoleAsync(user, "HR") && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Access Denied. Only HR personnel or Admins can log in.";
                return View(model);
            }

            // ✅ Check OTP cooldown
            var lastOtpTimeStr = HttpContext.Session.GetString("LoginOtpTime");
            if (lastOtpTimeStr != null && DateTime.TryParse(lastOtpTimeStr, out var lastOtpTime))
            {
                if ((DateTime.UtcNow - lastOtpTime).TotalSeconds < 60)
                {
                    TempData["ErrorMessage"] = "OTP already sent. Please wait before requesting a new one.";
                    return View(model);
                }
            }

            // ✅ Generate OTP
            var otp = GenerateSecureOtp();
            var phoneNumber = user.PhoneNumber;

            var sendData = new
            {
                sender_id = "PhilSMS",
                recipient = phoneNumber,
                message = $"Your OTP for login is: {otp}"
            };

            var token = "1179|EkuFgMmv7JYBgb0qwf5ZD6kZWKrvuqFyGHGb9g04";
            var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://app.philsms.com/api/v3/sms/send")
            {
                Headers = { { "Authorization", $"Bearer {token}" } },
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

            // ✅ Store OTP and info
            HttpContext.Session.SetString("Otp", otp);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("LoginOtpTime", DateTime.UtcNow.ToString());

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
            HttpContext.Session.Remove("LoginOtpTime"); // Only clear login-related keys
            HttpContext.Session.Remove("ResetOtpTime"); // Clear reset if needed too


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

        public IActionResult AccessDenied()
        {
            return View(); // You can create a simple view to show "Access Denied"
        }


        // Forgot Password View
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string OtpCode)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Email not found.";
                return View(model);
            }

            // ✅ If OTP input is empty, send OTP
            if (string.IsNullOrEmpty(OtpCode))
            {
                var otp = GenerateSecureOtp();
                var phoneNumber = user.PhoneNumber;

                var sendData = new
                {
                    sender_id = "PhilSMS",
                    recipient = phoneNumber,
                    message = $"Your OTP for password reset is: {otp}"
                };

                var token = "1179|EkuFgMmv7JYBgb0qwf5ZD6kZWKrvuqFyGHGb9g04";
                var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://app.philsms.com/api/v3/sms/send")
                {
                    Headers = { { "Authorization", $"Bearer {token}" } },
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

                HttpContext.Session.SetString("Otp", otp);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("ResetOtpTime", DateTime.UtcNow.ToString());

                TempData["OtpSent"] = true;
                TempData["SuccessMessage"] = "OTP sent successfully. Please check your phone.";

                return View(model);
            }
            else
            {
                // ✅ If OTP input is provided, verify it
                var expectedOtp = HttpContext.Session.GetString("Otp");
                var emailFromSession = HttpContext.Session.GetString("UserEmail");

                if (OtpCode == expectedOtp && model.Email == emailFromSession)
                {
                    TempData["SuccessMessageOTPVerified"] = "OTP verified.";
                    return RedirectToAction("ResetPassword", new { email = model.Email });
                }

                TempData["OtpSent"] = true; // keep showing OTP input
                TempData["ErrorMessage"] = "Invalid OTP. Please try again.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid reset link.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ForgotPassword");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessResetMessage"] = "Password reset successfully!";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }




    }
}
