using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_bancario_Sprint.Migrations
{
    /// <inheritdoc />
    public partial class AddSaldoApos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SaldoApos",
                table: "Transacoes",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaldoApos",
                table: "Transacoes");
        }
    }
}
