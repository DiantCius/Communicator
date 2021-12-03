using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class UpdateInvitationForRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invitations_AddressedUserId",
                table: "Invitations",
                column: "AddressedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ChildId",
                table: "Invitations",
                column: "ChildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Children_ChildId",
                table: "Invitations",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "ChildId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Persons_AddressedUserId",
                table: "Invitations",
                column: "AddressedUserId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Children_ChildId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Persons_AddressedUserId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_AddressedUserId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_ChildId",
                table: "Invitations");
        }
    }
}
