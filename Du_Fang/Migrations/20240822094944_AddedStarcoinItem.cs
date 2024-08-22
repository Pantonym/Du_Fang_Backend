using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Du_Fang.Migrations
{
    /// <inheritdoc />
    public partial class AddedStarcoinItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoinBalance",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoinBalance",
                table: "Accounts");
        }
    }
}
