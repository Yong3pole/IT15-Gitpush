using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IT15_TripoleMedelTijol.Models;
using IT15_TripoleMedelTijol.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware setup
app.UseAuthentication();
app.UseAuthorization();

// File serving
app.UseStaticFiles();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// 🔹 **Seed User & Reset Password in a Scoped Service**
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var user = await userManager.FindByEmailAsync("john.doe@example.com");
    if (user != null)
    {
        string newPassword = "Admin123!";
        string hashedPassword = userManager.PasswordHasher.HashPassword(user, newPassword);
        user.PasswordHash = hashedPassword;
        await userManager.UpdateAsync(user);
        Console.WriteLine("✅ Password reset successfully!");
    }
    else
    {
        Console.WriteLine("❌ User not found.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error pages in development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Custom error page for production
}



app.Run();
