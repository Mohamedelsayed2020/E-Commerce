using E_Commerce.Models;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            // check if we have the admin role, if not create it
            if (!await roleManager.RoleExistsAsync("admin"))
            {
                Console.WriteLine("Admin role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            // check if we have the seller role, if not create it
            if (!await roleManager.RoleExistsAsync("seller"))
            {
                Console.WriteLine("Seller role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            // check if we have the client role, if not create it
            if (!await roleManager.RoleExistsAsync("client"))
            {
                Console.WriteLine("Client role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            // check if we have at least one admin user or not
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                // admin user already exist =>exit
                Console.WriteLine("Admin user already exist => exit");
                return;
            }

            // create a default admin user
            var adminUser = new ApplicationUser
            {
                FirstNme = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now
            };
            string initialPassword = "Admin@123"; // You can change this to a more secure password
            var result = await userManager.CreateAsync(adminUser, initialPassword);
            if (result.Succeeded)
            {
                // assign the admin role to the user
                await userManager.AddToRoleAsync(adminUser, "admin");
                Console.WriteLine("Admin user created successfully with email:" + adminUser.Email);
            }

        }
    }
}
