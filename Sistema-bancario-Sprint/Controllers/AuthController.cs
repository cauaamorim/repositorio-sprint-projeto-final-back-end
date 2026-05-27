using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Sistema_bancario_Sprint.DTOs;
using Sistema_bancario_Sprint.models;
using Sistema_bancario_Sprint.Data;

namespace Sistema_bancario_Sprint.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public AuthController(IConfiguration config, AppDbContext db)
    {
        _config = config;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto, [FromQuery] string tipo = "corrente")
    {
        // basic creation - password not hashed for brevity (should hash in production)
        Conta conta = tipo.ToLower() switch
        {
            "empresarial" => new ContaEmpresarial { Nome = dto.Nome, Email = dto.Email, Senha = dto.Senha, /*Saldo initialized by default*/ },
            _ => new ContaCorrente { Nome = dto.Nome, Email = dto.Email, Senha = dto.Senha }
        };

        // Apply optional spending limit provided at registration
        conta.LimiteAtivo = dto.LimiteAtivo;
        conta.LimiteGasto = dto.LimiteGasto;

        _db.Contas.Add(conta);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Register), new { id = conta.Id }, new { conta.Id });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO dto)
    {
        var conta = _db.Contas.FirstOrDefault(c => c.Email == dto.Email && c.Senha == dto.Senha);
        if (conta == null) return Unauthorized("email ou senha incorretos");

        var token = GenerateToken(conta);
        return Ok(new { token, id = conta.Id });
    }

    private string GenerateToken(Conta conta)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, conta.Id.ToString()),
            new Claim(ClaimTypes.Name, conta.Email)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
