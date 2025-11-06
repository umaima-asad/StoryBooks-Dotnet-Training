namespace StoryBooks.Application.Interfaces
{
    public interface ITenantProvider
    {
        Task<int> GetTenantId();
    }
}
