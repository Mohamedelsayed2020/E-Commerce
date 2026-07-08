using E_Commerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ApplicarionDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicarionDbContext(DbContextOptions options): base(options)
        {
                
        }

        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
}
