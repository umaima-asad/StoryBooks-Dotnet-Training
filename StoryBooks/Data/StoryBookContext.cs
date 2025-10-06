using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StoryBooks.Models;
namespace StoryBooks.Data
{
    public class StoryBookContext : IdentityDbContext
    {
        public StoryBookContext(DbContextOptions<StoryBookContext> options) : base(options)
        {
        }
        public DbSet<StoryBook> StoryBooks { get; set; } = null!;
        public DbSet<UsersModel> Users { get; set; }
    }
}
