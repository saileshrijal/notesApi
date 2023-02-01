using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Notes.Config;
using Notes.Dtos;
using Notes.Models;
using Notes.Services.interfaces;

namespace Notes.Services.implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthService(UserManager<ApplicationUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        public async Task<AuthResult> Login(LoginDto loginDto)
        {
            var existingUser = await _userManager.FindByNameAsync(loginDto.Username!);
            if (existingUser == null)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>{
                            "Invalid Username"
                        }
                };
            }
            var checkPassword = await _userManager.CheckPasswordAsync(existingUser, loginDto.Password!);
            if (!checkPassword)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>{
                            "Password do not match"
                        }
                };
            }
            return new AuthResult
            {
                Success = true
            };
        }

        public async Task<AuthResult> Register(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username!);
            if (existingUser != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{
                            "Email already in use"
                        }
                };
            }
            var newUser = new ApplicationUser
            {
                UserName = registerDto.Username,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
            };
            var isCreated = await _userManager.CreateAsync(newUser, registerDto.Password!);
            if (isCreated.Succeeded)
            {
                return new AuthResult
                {
                    Success = true,
                };
            }
            else
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = isCreated.Errors.Select(x => x.Description).ToList()
                };
            }
        }
    }
}
