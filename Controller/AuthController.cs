using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DSW_3P_PR01.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace DSW_3P_PR01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto? login)
    {
        if (login == null || string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Password))
        {
            return BadRequest(new { mensaje = "El usuario y la contraseña son requeridos." });
        }

        // Usuario temporal para la práctica
        if (login.Usuario != "admin" || login.Password != "QuestLog123!")
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }

        var jwt = _configuration.GetSection("Jwt");
        var keyString = jwt["Key"];

        if (string.IsNullOrWhiteSpace(keyString))
        {
            return StatusCode(500, new { mensaje = "No se configuró Jwt:Key en la aplicación." });
        }

        byte[] key;
        try
        {
            key = Convert.FromBase64String(keyString);
        }
        catch (FormatException)
        {
            return StatusCode(500, new { mensaje = "La clave Jwt:Key no tiene un formato Base64 válido." });
        }

        int expirationMinutes = 30;
        if (int.TryParse(jwt["ExpirationMinutes"], out int parsedExp) && parsedExp > 0)
        {
            expirationMinutes = parsedExp;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Usuario),
            new Claim(ClaimTypes.Role, "Administrador")
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"] ?? "DSW_3P_EJ01",
            audience: jwt["Audience"] ?? "DSW_3P_EJ01_Clients",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            access_token = tokenString,
            token_type = "Bearer",
            expires_in = expirationMinutes * 60
        });
    }
}
