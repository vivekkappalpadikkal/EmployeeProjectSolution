using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeProjectApi.Dtos;

namespace EmployeeProjectApi.Controllers.V2;
[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    public AuthController(IConfiguration cfg) => _cfg = cfg;

    // simple in-memory users
    private static readonly Dictionary<string, (string Pwd, string Role)> Users = new()
    {
        ["admin@example.com"] = ("admin123", "Admin"),
        ["user@example.com"] = ("user123", "User")
    };

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (!Users.TryGetValue(dto.Email, out var user) || user.Pwd != dto.Password)
            return Unauthorized("Invalid credentials");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                        int.Parse(_cfg["Jwt:ExpirationMinutes"]!)),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}
