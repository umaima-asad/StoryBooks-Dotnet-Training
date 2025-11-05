using Microsoft.AspNetCore.Identity;
namespace StoryBooks.Domain.Models
{
    public class UsersModel : IdentityUser
    {
        public int TenantID { get; set; }
        public string Fullname { get; set; }
    }
}