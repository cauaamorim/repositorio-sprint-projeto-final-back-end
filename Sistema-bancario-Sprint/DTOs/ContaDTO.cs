using System.ComponentModel.DataAnnotations;

namespace Sistema_bancario_Sprint.DTOs;

public abstract class ContaDTO
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty; // Para o Login/JWT

    [Required]
    public string Senha { get; set; } = string.Empty; // Para o Login

    [Required]
    public double Saldo { get; protected set; } // protected set para filhas alterarem
}
