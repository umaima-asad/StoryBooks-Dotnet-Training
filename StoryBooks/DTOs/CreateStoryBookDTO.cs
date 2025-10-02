namespace StoryBooks.DTOs
{
    public class CreateStoryBookDTO
    {
        public string BookName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public IFormFile? Cover { get; set; }

    }
}
