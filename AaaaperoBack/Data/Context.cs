using Microsoft.EntityFrameworkCore;

namespace AaaaperoBack.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}
        
    }
}