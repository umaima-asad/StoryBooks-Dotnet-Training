namespace StoryBooks.Application.DTOs
{
    public class StoryBookDTO
    {
        public string BookName { get; set; } = string.Empty;    
        public string Author { get; set; } = string.Empty;
        public string? Cover { get; set; }
    }
}
