using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseTemplate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedBySsoId",
                table: "StaffRequest");

            migrationBuilder.DropColumn(
                name: "RequestedBySsoId",
                table: "StaffRequest");

            migrationBuilder.AddColumn<int>(
                name: "AcceptedByAppUserId",
                table: "StaffRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestedByAppUserId",
                table: "StaffRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StaffRequest_AcceptedByAppUserId",
                table: "StaffRequest",
                column: "AcceptedByAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffRequest_RequestedByAppUserId",
                table: "StaffRequest",
                column: "RequestedByAppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRequest_AppUser_AcceptedByAppUserId",
                table: "StaffRequest",
                column: "AcceptedByAppUserId",
                principalTable: "AppUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRequest_AppUser_RequestedByAppUserId",
                table: "StaffRequest",
                column: "RequestedByAppUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRequest_AppUser_AcceptedByAppUserId",
                table: "StaffRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffRequest_AppUser_RequestedByAppUserId",
                table: "StaffRequest");

            migrationBuilder.DropIndex(
                name: "IX_StaffRequest_AcceptedByAppUserId",
                table: "StaffRequest");

            migrationBuilder.DropIndex(
                name: "IX_StaffRequest_RequestedByAppUserId",
                table: "StaffRequest");

            migrationBuilder.DropColumn(
                name: "AcceptedByAppUserId",
                table: "StaffRequest");

            migrationBuilder.DropColumn(
                name: "RequestedByAppUserId",
                table: "StaffRequest");

            migrationBuilder.AddColumn<string>(
                name: "AcceptedBySsoId",
                table: "StaffRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedBySsoId",
                table: "StaffRequest",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
