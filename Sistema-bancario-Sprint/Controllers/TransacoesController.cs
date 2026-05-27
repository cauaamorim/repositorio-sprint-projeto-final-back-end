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
        // 1. TRAVA PARA LETRAS: Se o usuário enviar letras no campo numérico, o .NET invalida o modelo aqui
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Dados inválidos. Certifique-se de que o valor é numérico." });

        // 2. TRAVA PARA VALORES NEGATIVOS OU ZERO
        if (dto.Valor <= 0)
            return BadRequest(new { message = "O valor do depósito deve ser maior que zero." });

        // Apenas o dono da conta pode depositar na própria conta
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
        // 1. TRAVA PARA LETRAS: Bloqueia caracteres de texto enviados no lugar do número
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Dados inválidos. Certifique-se de que o valor é numérico." });

        // 2. TRAVA PARA VALORES NEGATIVOS OU ZERO
        if (dto.Valor <= 0)
            return BadRequest(new { message = "O valor do saque deve ser maior que zero." });

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