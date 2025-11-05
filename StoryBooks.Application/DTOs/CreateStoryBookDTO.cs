using Microsoft.AspNetCore.Http;

namespace StoryBooks.Application.DTOs
{
    public class CreateStoryBookDTO
    {
        public int TenantID { get; set; }
        public string BookName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public IFormFile? Cover { get; set; }

    }
}
