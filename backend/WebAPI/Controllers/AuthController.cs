using Auth.Services;
using Microsoft.AspNetCore.Mvc;
using static Auth.Dtos.Dtos;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto, CancellationToken ct)
        {
            await authService.RegisterAsync(dto, ct);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto, CancellationToken ct)
        {
            var token = await authService.LoginAsync(dto, ct);
            return Ok(new { token });
        }
    }
}
