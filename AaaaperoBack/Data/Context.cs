using AaaaperoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AaaaperoBack.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}
        public DbSet<User> User {get; set;}
        
        public DbSet<Candidate> Candidate { get; set; }
        
        public DbSet<Employer> Employer { get; set; }
        
        public DbSet<Message> Message { get; set; }
        
        public DbSet<Conversation> Conversation { get; set; }
    }
}