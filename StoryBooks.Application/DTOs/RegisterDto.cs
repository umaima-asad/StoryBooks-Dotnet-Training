namespace StoryBooks.Application.DTOs
{
    public class RegisterDTO
    {
        public int TenantID { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
    }
}
