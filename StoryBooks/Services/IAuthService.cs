using StoryBooks.DTOs;
using StoryBooks.Models;
namespace StoryBooks.Services
{
    public interface IAuthService
    {
        Task<UsersModel?> RegisterAsync(RegisterDto User);
        Task<string> LoginAsync(LoginDto User);
    }
}
