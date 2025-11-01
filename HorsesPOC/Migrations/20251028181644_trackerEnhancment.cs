using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesPOC.Migrations
{
    /// <inheritdoc />
    public partial class trackerEnhancment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Horses_Stables_StableID",
                table: "Horses");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Stables_StableID",
                table: "Trainees");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingTracker_Trainees_TraineeId",
                table: "TrainingTracker");

            migrationBuilder.AddColumn<Guid>(
                name: "HorseId",
                table: "TrainingTracker",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TrainingTracker_HorseId",
                table: "TrainingTracker",
                column: "HorseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Horses_Stables_StableID",
                table: "Horses",
                column: "StableID",
                principalTable: "Stables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Stables_StableID",
                table: "Trainees",
                column: "StableID",
                principalTable: "Stables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingTracker_Horses_HorseId",
                table: "TrainingTracker",
                column: "HorseId",
                principalTable: "Horses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingTracker_Trainees_TraineeId",
                table: "TrainingTracker",
                column: "TraineeId",
                principalTable: "Trainees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Horses_Stables_StableID",
                table: "Horses");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Stables_StableID",
                table: "Trainees");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingTracker_Horses_HorseId",
                table: "TrainingTracker");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingTracker_Trainees_TraineeId",
                table: "TrainingTracker");

            migrationBuilder.DropIndex(
                name: "IX_TrainingTracker_HorseId",
                table: "TrainingTracker");

            migrationBuilder.DropColumn(
                name: "HorseId",
                table: "TrainingTracker");

            migrationBuilder.AddForeignKey(
                name: "FK_Horses_Stables_StableID",
                table: "Horses",
                column: "StableID",
                principalTable: "Stables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Stables_StableID",
                table: "Trainees",
                column: "StableID",
                principalTable: "Stables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingTracker_Trainees_TraineeId",
                table: "TrainingTracker",
                column: "TraineeId",
                principalTable: "Trainees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
