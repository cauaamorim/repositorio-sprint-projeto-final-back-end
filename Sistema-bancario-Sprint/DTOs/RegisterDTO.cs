namespace Sistema_bancario_Sprint.DTOs;

public class RegisterDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    // Optional spending limit provided at registration
    public bool LimiteAtivo { get; set; } = false;
    public decimal LimiteGasto { get; set; } = 0m;
}
