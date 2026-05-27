using Sistema_bancario_Sprint.models;
using System.ComponentModel.DataAnnotations;

namespace Sistema_bancario_Sprint.DTOs;

public class TransacaoDTO
{
    [Required]
    public int ContaId { get; set; }
   

    [Required]
    public double Valor { get; set; }

    public DateTime Data { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; }

    public decimal SaldoApos { get; set; }
    public decimal TaxaAplicada { get; set; }
}
