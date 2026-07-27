using FirstMVCWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstMVCWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

     
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<StudentDetails> Courses { get; set; }
    }
}