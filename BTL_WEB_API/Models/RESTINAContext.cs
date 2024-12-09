using Microsoft.EntityFrameworkCore;
using BTL_WEB_API.Models;

namespace BTL_WEB_API.Models
{
    public class RESTINAContext:DbContext
    {
        public RESTINAContext(DbContextOptions<RESTINAContext> options):base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
