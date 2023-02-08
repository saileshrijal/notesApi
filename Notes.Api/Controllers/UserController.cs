using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notes.Config;
using Notes.Dtos;
using Notes.Models;
using Notes.Services.interfaces;
using Notes.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Notes.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly IAuthService _authService;
        public UserController(UserManager<ApplicationUser> userManager,
                                    IOptionsMonitor<JwtConfig> optionsMonitor,
                                    IAuthService authService)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _authService = authService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) { return BadRequest("Invalid Payload"); }
            try
            {
                var loginDdto = new LoginDto()
                {
                    Username = vm.Username,
                    Password = vm.Password,
                };
                var authResult = await _authService.Login(loginDdto);
                if (!authResult.Success) { return BadRequest(authResult.Errors); }
                var existingUser = await _userManager.FindByNameAsync(vm.Username!);
                authResult.Token = GenerateToken(existingUser!);
                return Ok(authResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationVM vm)
        {
            if (ModelState.IsValid)
            {
                var registerDto = new RegisterDto()
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Username = vm.Username,
                    Password = vm.Password
                };
                var authResult = await _authService.Register(registerDto);
                if (!authResult.Success) { return BadRequest(authResult.Errors); }
                var existingUser = await _userManager.FindByNameAsync(vm.Username!);
                authResult.Token = GenerateToken(existingUser!);
                return Ok(authResult);
            }
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LoggedInUser()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await _userManager.FindByNameAsync(username!);
            var vm = new LoggedInUserVM
            {
                FirstName = loggedInUser!.FirstName,
                LastName = loggedInUser.LastName,
                Username = loggedInUser.UserName
            };
            return Ok(vm);
        }

        private string GenerateToken(ApplicationUser newUser)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret!);
            var jwtDescreptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", newUser.Id),
                    new Claim(JwtRegisteredClaimNames.Name, newUser.UserName!),
                    new Claim(JwtRegisteredClaimNames.Sub, newUser.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),

                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };
            var token = jwtHandler.CreateToken(jwtDescreptor);
            var jwtToken = jwtHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
