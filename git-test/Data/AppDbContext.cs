using git_test.Models;
using Microsoft.EntityFrameworkCore;

namespace git_test.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {   
        }

        public DbSet<Student> Students { get; set; }
    }
}
