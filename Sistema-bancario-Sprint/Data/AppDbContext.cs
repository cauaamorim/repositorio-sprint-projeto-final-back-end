using Microsoft.EntityFrameworkCore;
using Sistema_bancario_Sprint.models;

namespace Sistema_bancario_Sprint.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Conta> Contas { get; set; }
    public DbSet<ContaCorrente> ContasCorrente { get; set; }
    public DbSet<ContaEmpresarial> ContasEmpresariais { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
}
