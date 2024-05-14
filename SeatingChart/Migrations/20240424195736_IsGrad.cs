using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatingChart.Migrations
{
    /// <inheritdoc />
    public partial class IsGrad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isGrad",
                table: "Student",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isGrad",
                table: "Student");
        }
    }
}
