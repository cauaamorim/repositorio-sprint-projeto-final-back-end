using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_bancario_Sprint.models;

public enum TipoTransacao
{
    Deposito,
    Saque
}

public class Transacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ContaId { get; set; }


    [Required]
    public double Valor { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; }

    public double TaxaAplicada { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    public decimal SaldoApos { get; set; } // saldo da conta após a transação
    // Navigation property
    [ForeignKey(nameof(ContaId))]
    public Conta? Conta { get; set; }
}
