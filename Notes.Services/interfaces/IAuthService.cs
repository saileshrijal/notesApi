using Notes.Config;
using Notes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Services.interfaces
{
    public interface IAuthService
    {
        public Task<AuthResult> Login(LoginDto loginDto);
        public Task<AuthResult> Register(RegisterDto registerDto);
    }
}
