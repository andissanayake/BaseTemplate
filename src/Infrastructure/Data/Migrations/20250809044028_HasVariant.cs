using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseTemplate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HasVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasVariant",
                table: "Item",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasVariant",
                table: "Item");
        }
    }
}
