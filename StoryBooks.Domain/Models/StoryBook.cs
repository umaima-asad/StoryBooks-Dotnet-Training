namespace StoryBooks.Domain.Models
{
    public class StoryBook
    {
        public int Id { get; set; }
        public required string BookName { get; set; }
        public required string Author { get; set; }
        public string? Cover { get; set; }
    }
}
