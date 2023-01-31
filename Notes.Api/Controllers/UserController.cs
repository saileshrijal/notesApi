using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notes.Api.Config;
using Notes.Models;
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
        public UserController(UserManager<ApplicationUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) { return BadRequest("Invalid Payload"); }
            try
            {
                var existingUser = await _userManager.FindByNameAsync(vm.Username!);
                if (existingUser == null) { return BadRequest("Invalid Username"); }
                var checkPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password!);
                if (!checkPassword) { return BadRequest("Invalid Username or Password"); }
                var jwtToken = GenerateToken(existingUser);
                return Ok(new AuthResult
                {
                    Token = jwtToken,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationVM vm)
        {
            // validate
            if (ModelState.IsValid)
            {
                //check email already exists
                var existingUser = await _userManager.FindByNameAsync(vm.Username!);
                if (existingUser != null)
                {
                    return BadRequest(new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>{
                            "Email already in use"
                        }
                    });
                }
                //create user
                var newUser = new ApplicationUser
                {
                    UserName = vm.Username,
                    FirstName= vm.FirstName,
                    LastName= vm.LastName,
                };
                var isCreated = await _userManager.CreateAsync(newUser, vm.Password!);
                if (isCreated.Succeeded)
                {
                    return Ok(new AuthResult
                    {
                        Success = true,
                        Token = GenerateToken(newUser)
                    });
                }
                else
                {
                    return BadRequest(new AuthResult
                    {
                        Success = false,
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    });
                }
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
