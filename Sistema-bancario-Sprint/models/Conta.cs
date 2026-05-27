using System.ComponentModel.DataAnnotations;

namespace Sistema_bancario_Sprint.models;


public abstract class Conta
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty; // Para o Login/JWT

    [Required]
    public string Senha { get; set; } = string.Empty; // Para o Login

    [Required]
    public double Saldo { get; protected set; } // protected set para filhas alterarem

    public bool LimiteAtivo { get; set; } = false;
    public decimal LimiteGasto { get; set; } = 0;


    // Methods to update saldo safely from services
    public void Creditar(double valor)
    {
        if (valor <= 0) return;
        Saldo = Math.Round(Saldo + valor, 2);
    }

    public bool Debitar(double valor)
    {
        if (valor <= 0) return false;
        var amount = Math.Round(valor, 2);
        if (Saldo < amount) return false;
        Saldo = Math.Round(Saldo - amount, 2);
        return true;
    }
  
}


