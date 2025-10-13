namespace StoryBooks.Application.Interfaces
{
    public interface IRedisCacheService
    {
        T? GetData<T>(string key);
        void SetData<T>(string key, T value);
    }
}
