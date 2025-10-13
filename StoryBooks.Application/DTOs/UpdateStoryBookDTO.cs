using Microsoft.AspNetCore.Http;
namespace StoryBooks.Application.DTOs
{
    public class UpdateStoryBookDTO
    {
        public string? BookName { get; set; } 
        public string? Author { get; set; }
        public IFormFile? Cover { get; set; }
    }
}
