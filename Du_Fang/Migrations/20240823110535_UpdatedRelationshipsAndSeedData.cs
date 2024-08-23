using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Du_Fang.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationshipsAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Statuses_AccountStatusId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AccountStatusId",
                table: "Accounts",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_AccountStatusId",
                table: "Accounts",
                newName: "IX_Accounts_StatusId");

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "StatusId", "AnnualInterestRate", "StatusName", "TotalAmountCriteria", "TransactionFee", "TransactionsCriteria" },
                values: new object[,]
                {
                    { 1, 0.01, "Active", 0m, 1m, 0 },
                    { 2, 0.02, "Silver", 1000m, 0.5m, 10 },
                    { 3, 0.029999999999999999, "Gold", 10000m, 0m, 100 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Statuses_StatusId",
                table: "Accounts",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Statuses_StatusId",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Accounts",
                newName: "AccountStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_StatusId",
                table: "Accounts",
                newName: "IX_Accounts_AccountStatusId");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Statuses_AccountStatusId",
                table: "Accounts",
                column: "AccountStatusId",
                principalTable: "Statuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
