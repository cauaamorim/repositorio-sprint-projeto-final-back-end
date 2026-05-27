namespace Sistema_bancario_Sprint.Services;

public enum WithdrawResult
{
    Success = 0,
    ContaNaoEncontrada = 1,
    SaldoInsuficiente = 2,
    LimiteAtingido = 3
}
