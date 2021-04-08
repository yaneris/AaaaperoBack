using AaaaperoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AaaaperoBack.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}
        public DbSet<User> User {get; set;}
        
        public DbSet<Advertisement> Advertisement { get; set; }
    }
}