using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StoryBooks.Data;
using StoryBooks.DTOs;
using StoryBooks.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoryBooks.Services
{
    public class AuthService(IConfiguration _config , UserManager<UsersModel> _userManager, SignInManager<UsersModel> _signInManager) : IAuthService
    {

        public async Task<string> LoginAsync(LoginDto Userdto)
        {
            var user = await _userManager.FindByEmailAsync(Userdto.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, Userdto.Password, false);
            if (!result.Succeeded) return null;

            var token = GenerateJwtToken(user);
            return token;
        }

        public async Task<UsersModel?> RegisterAsync(RegisterDto User)
        {
            var user = new UsersModel
            {
                UserName = User.Email,
                Email = User.Email,
                Fullname = User.Fullname
            };

            var result = await _userManager.CreateAsync(user, User.Password);

            if (!result.Succeeded)
                return user;
            else return null;
        }
        private string GenerateJwtToken(UsersModel user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("uid", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Fullname ?? user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config.GetValue<string>("AppSettings:Issuer"),
                audience: _config.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
