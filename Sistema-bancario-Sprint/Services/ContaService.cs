using Microsoft.EntityFrameworkCore;
using Sistema_bancario_Sprint.Data;
using Sistema_bancario_Sprint.models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_bancario_Sprint.Services;

public class ContaService : IContaService
{
    private readonly AppDbContext _db;

    public ContaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Conta> CreateAsync(Conta conta)
    {
        _db.Contas.Add(conta);
        await _db.SaveChangesAsync();
        return conta;
    }

    public async Task<IEnumerable<Conta>> GetAllAsync()
    {
        return await _db.Contas.AsNoTracking().ToListAsync();
    }

    public async Task<Conta?> GetByIdAsync(int id)
    {
        return await _db.Contas.FindAsync(id);
    }

    public async Task<bool> DepositAsync(int contaId, double valor)
    {
        var conta = await _db.Contas.FindAsync(contaId);
        if (conta == null) return false;
        // No taxa para depósito neste exemplo
        conta.Creditar(valor);

        var transacao = new Transacao { ContaId = contaId, Valor = valor, Tipo = TipoTransacao.Deposito,Data=DateTime.UtcNow, SaldoApos = (decimal)Math.Round((decimal)conta.Saldo, 2), TaxaAplicada = 0 };
        _db.Transacoes.Add(transacao);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<WithdrawResult> WithdrawAsync(int contaId, double valor)
    {
        var conta = await _db.Contas.FindAsync(contaId);
        if (conta == null) return WithdrawResult.ContaNaoEncontrada;

        // NOTE: Limite agora funciona como um saldo mínimo (floor).
        // Ou seja, se o limite estiver ativo, não é mais proibido um saque
        // somente por ultrapassar um valor por operação. Em vez disso,
        // impedimos o saque quando o saldo após a operação ficaria abaixo
        // do `LimiteGasto` configurado.

        double taxa = 0;
        if (conta is ContaCorrente)
        {
            // No exemplo atual preferimos não aplicar taxa fixa para saque em conta corrente
            taxa = 0.0;
        }
        else if (conta is ContaEmpresarial)
        {
            taxa = Math.Round(valor * 0.01, 2); // 1% para empresarial
        }
        else
        {
            taxa = 0; // poupança sem taxa neste exemplo
        }

        // Use arredondamento para evitar problemas de precisão
        var total = Math.Round(valor + taxa, 2);
        if (conta.Saldo < total) return WithdrawResult.SaldoInsuficiente;

        // If spending limit is active, ensure resulting balance >= LimiteGasto
        if (conta.LimiteAtivo)
        {
            // Convert types consistently: Saldo (double) and LimiteGasto (decimal)
            var resultingBalance = Math.Round(conta.Saldo - total, 2);
            if (resultingBalance < (double)conta.LimiteGasto)
            {
                // Deny withdrawal because it would drop balance below configured limit
                return WithdrawResult.LimiteAtingido;
            }
        }

        // Debitar o valor + taxa
        var success = conta.Debitar(total);
        if (!success) return WithdrawResult.SaldoInsuficiente;

        var transacao = new Transacao { ContaId = contaId, Valor = Math.Round(valor, 2), Data = DateTime.UtcNow, Tipo = TipoTransacao.Saque, SaldoApos = (decimal)Math.Round((decimal)conta.Saldo, 2), TaxaAplicada = taxa };
        _db.Transacoes.Add(transacao);

        await _db.SaveChangesAsync();
        return WithdrawResult.Success;
    }



    public async Task<bool> UpdateLimitAsync(int contaId, bool limiteAtivo, decimal limiteGasto)
    {
        var conta = await _db.Contas.FindAsync(contaId);
        if (conta == null) return false;

        conta.LimiteAtivo = limiteAtivo;
        conta.LimiteGasto = limiteGasto;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Transacao>> GetTransactionsAsync(int contaId, int page = 1, int pageSize = 50)
    {
        return await _db.Transacoes
            .Where(t => t.ContaId == contaId)
            .OrderByDescending(t => t.Data)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
}
