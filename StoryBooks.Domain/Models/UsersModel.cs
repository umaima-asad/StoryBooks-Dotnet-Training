using Microsoft.AspNetCore.Identity;
namespace StoryBooks.Domain.Models
{
    public class UsersModel : IdentityUser
    {
        public string Fullname { get; set; }
    }
}