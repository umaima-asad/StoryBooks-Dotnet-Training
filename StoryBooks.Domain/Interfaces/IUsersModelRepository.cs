namespace StoryBooks.Domain.Interfaces
{
    public interface IUsersModelRepository
    {
        Task<int> GetTenantIdByUserIdAsync(string UserID);
    }
}
