using CrudBackend_1_.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudBackend_1_.Data
{
    public class AppDBContextcs(DbContextOptions<AppDBContextcs> options) : DbContext(options)
    {
       public DbSet<User> Users { get; set; }
       public DbSet<Product> Products { get; set; }
    }
}
