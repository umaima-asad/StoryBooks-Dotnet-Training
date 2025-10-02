using Microsoft.AspNetCore.Identity;

namespace StoryBooks.Models
{
    public class UsersModel : IdentityUser
    {
        public string Fullname { get; set; }
    }
}
