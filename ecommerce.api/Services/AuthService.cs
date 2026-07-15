using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ecommerce.api;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        var emailExists = await _context.Users.AnyAsync(U => U.Email == registerDto.Email);

        if (emailExists)
        {
            return null;
        }

        var user = new User
        {
            FullName = registerDto.FullName,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return null;
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

        if (!passwordValid)
        {
            return null;
        }

        var token = GenerateJwtToken(user);


        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
       {
           new Claim( ClaimTypes.NameIdentifier, user.Id.ToString()),
           new Claim(ClaimTypes.Name, user.Email.ToString()),
           new Claim(ClaimTypes.Role, user.Role)
       };
        var key = new SymmetricSecurityKey(
         Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

        var credentials = new SigningCredentials
        (
        key,
        SecurityAlgorithms.HmacSha256
        );

        var expiresInMinutes = Convert.ToDouble(
            _configuration["Jwt:ExpiresInMinutes"]
        );

        var token = new JwtSecurityToken(
            issuer : _configuration["Jwt : Issuer"],
            audience: _configuration["Jwt : Audience"],
            claims : claims,
            expires : DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials : credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
