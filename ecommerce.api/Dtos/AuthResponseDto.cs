using System;

namespace ecommerce.api;

public class AuthResponseDto
{
    public string Token {get; set;} = string.Empty;
    public string Email {get; set; } = string.Empty;
    public string Role {get; set;} = string.Empty;

}
