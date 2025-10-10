public sealed class PlaceholderService
{
    private readonly HttpClient _client;
    private static readonly Random _random = new();

    public PlaceholderService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> GetPostAsync(int id)
    {
        // Simulate transient error 50% of the time
        if (_random.NextDouble() < 0.5)
        {
            throw new HttpRequestException("API unreponsive!");
        }

        var response = await _client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{id}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
