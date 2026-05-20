using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ApplicarionDbContext : DbContext
    {
        public ApplicarionDbContext(DbContextOptions options): base(options)
        {
                
        }

        public DbSet<Models.Product> Products { get; set; }

    }
}
