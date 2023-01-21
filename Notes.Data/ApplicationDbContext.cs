using Microsoft.EntityFrameworkCore;
using Notes.Models;

namespace Notes.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option):base(option)
        {
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Note> Note { get; set; }
    }
}
