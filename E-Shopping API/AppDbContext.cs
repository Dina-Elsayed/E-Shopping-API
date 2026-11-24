using E_Shopping_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Shopping_API
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Cart> Carts { get; set; }


        public DbSet<User> Users { get; set; }
    }
}
