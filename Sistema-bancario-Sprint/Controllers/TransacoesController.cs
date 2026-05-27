using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Sistema_bancario_Sprint.DTOs;
using Sistema_bancario_Sprint.Services;

namespace Sistema_bancario_Sprint.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly IContaService _service;

    public TransacoesController(IContaService service)
    {
        _service = service;
    }

    [HttpPost("depositar")]
    [Authorize]
    public async Task<IActionResult> Depositar([FromBody] TransacaoDTO dto)
    {
        // only owner can deposit to their own account
        var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claimId, out var ownerId) || ownerId != dto.ContaId)
            return Forbid();

        var ok = await _service.DepositAsync(dto.ContaId, dto.Valor);
        if (!ok) return BadRequest(new { message = "Impossível realizar o depósito." });
        return Ok(new { message = "Depósito realizado." });
    }

    [HttpPost("sacar")]
    [Authorize]
    public async Task<IActionResult> Sacar([FromBody] TransacaoDTO dto)
    {
        var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claimId, out var ownerId) || ownerId != dto.ContaId)
            return Forbid();

        var result = await _service.WithdrawAsync(dto.ContaId, dto.Valor);
        return result switch
        {
            Services.WithdrawResult.Success => Ok(new { message = "Saque realizado." }),
            Services.WithdrawResult.ContaNaoEncontrada => NotFound(new { message = "Conta não encontrada." }),
            Services.WithdrawResult.SaldoInsuficiente => BadRequest(new { message = "Saldo insuficiente." }),
            Services.WithdrawResult.LimiteAtingido => BadRequest(new { message = "Limite atingido." }),
            _ => BadRequest(new { message = "Erro ao processar saque." })
        };
    }
}
