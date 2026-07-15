using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if(result == null)
            {
                return BadRequest("Email alresdy exists");
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null )
            {
                return Unauthorized("Invalid Email or password");
            }
            return Ok(result);
        }

    }
}
