using System;

namespace ecommerce.api;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
}
