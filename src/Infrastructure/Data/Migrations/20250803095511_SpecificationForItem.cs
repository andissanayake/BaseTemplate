using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseTemplate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SpecificationForItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecificationId",
                table: "Item",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AppUser",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffInvitationRole_StaffInvitationId",
                table: "StaffInvitationRole",
                column: "StaffInvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_SpecificationId",
                table: "Item",
                column: "SpecificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Specification_SpecificationId",
                table: "Item",
                column: "SpecificationId",
                principalTable: "Specification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffInvitationRole_StaffInvitation_StaffInvitationId",
                table: "StaffInvitationRole",
                column: "StaffInvitationId",
                principalTable: "StaffInvitation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Specification_SpecificationId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffInvitationRole_StaffInvitation_StaffInvitationId",
                table: "StaffInvitationRole");

            migrationBuilder.DropIndex(
                name: "IX_StaffInvitationRole_StaffInvitationId",
                table: "StaffInvitationRole");

            migrationBuilder.DropIndex(
                name: "IX_Item_SpecificationId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "SpecificationId",
                table: "Item");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AppUser",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
