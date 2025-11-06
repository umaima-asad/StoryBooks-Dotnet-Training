using Microsoft.EntityFrameworkCore;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;
using StoryBooks.Infrastructure.Data;


namespace UserModels.Infrastructure.Repositories
{
    public class UsersModelRepository : IUsersModelRepository
    {
        private readonly StoryBookContext _context;

        public UsersModelRepository(StoryBookContext context)
        {
            _context = context;
        }
        public async Task<int> GetTenantIdByUserIdAsync(string UserID)
        {
            var userModel = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(um => um.Id== UserID);
            return userModel != null ? userModel.TenantID : 0;
        }
    }
}
