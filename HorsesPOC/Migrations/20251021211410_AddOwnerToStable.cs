using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesPOC.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerToStable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Stables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Stables_OwnerId",
                table: "Stables",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stables_users_OwnerId",
                table: "Stables",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stables_users_OwnerId",
                table: "Stables");

            migrationBuilder.DropIndex(
                name: "IX_Stables_OwnerId",
                table: "Stables");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Stables");
        }
    }
}
