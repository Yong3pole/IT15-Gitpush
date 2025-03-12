using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IT15_TripoleMedelTijol.Models;
using IT15_TripoleMedelTijol.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Register database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Register Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 🔹 Register HttpClient for dependency injection
builder.Services.AddHttpClient();

// 🔹 Enable Sessions ✅ FIXED
builder.Services.AddSession();

// 🔹 Register MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 🔹 Middleware setup
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Enable Sessions ✅ FIXED
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// 🔹 Seed roles and admin user
await SeedRolesAndAdminUserAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}



// ✅ Move seeding logic to a separate async method
async Task SeedRolesAndAdminUserAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure roles exist
        string[] roles = { "Admin", "HR", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Ensure admin user exists
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumber = "+639055501894" // ✅ Insert phone number
            };

            var createUserResult = await userManager.CreateAsync(newUser, "Admin123!");
            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "Admin");
                Console.WriteLine("✅ Admin user created successfully!");
            }
            else
            {
                Console.WriteLine("❌ Failed to create admin user: " +
                    string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine("✅ Admin user already exists.");
        }
    }
}

app.Run();
