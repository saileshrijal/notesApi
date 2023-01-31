using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.Models;

namespace Notes.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option):base(option)
        {
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Note> Note { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
