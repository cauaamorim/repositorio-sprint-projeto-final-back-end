using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_bancario_Sprint.Migrations
{
    /// <inheritdoc />
    public partial class AddSpendingLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LimiteAtivo",
                table: "Contas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LimiteGasto",
                table: "Contas",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimiteAtivo",
                table: "Contas");

            migrationBuilder.DropColumn(
                name: "LimiteGasto",
                table: "Contas");
        }
    }
}
