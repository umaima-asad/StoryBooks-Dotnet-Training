using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoryBooks.Application.Services;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;

namespace StoryBooks.Infrastructure.Data
{
    public class StoryBookContext : IdentityDbContext<UsersModel>
    {
        private readonly ITenantProvider _tenantProvider;
        public StoryBookContext(DbContextOptions<StoryBookContext> options, ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
        }
        public DbSet<StoryBook> StoryBooks { get; set; } = null!;
        public DbSet<UsersModel> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StoryBook>(builder =>
            {
                builder.HasIndex(sb => sb.TenantID);
                builder.HasQueryFilter(sb => sb.TenantID == _tenantProvider.GetTenantId());  
            });
            modelBuilder.Entity<UsersModel>(builder =>
            {
                builder.HasIndex(u => u.TenantID);
                builder.HasQueryFilter(u => u.TenantID == _tenantProvider.GetTenantId());
            });
        }
    }
}
