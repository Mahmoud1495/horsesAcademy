using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesPOC.Migrations
{
    /// <inheritdoc />
    public partial class TrainingTrackerupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActualTrainingInMin",
                table: "TrainingTracker",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualTrainingInMin",
                table: "TrainingTracker");
        }
    }
}
