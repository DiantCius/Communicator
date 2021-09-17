using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class UpdateActivity2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Persons_AuthorPersonId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_AuthorPersonId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "AuthorPersonId",
                table: "Activities");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AuthorId",
                table: "Activities",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Persons_AuthorId",
                table: "Activities",
                column: "AuthorId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Persons_AuthorId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_AuthorId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Activities");

            migrationBuilder.AddColumn<int>(
                name: "AuthorPersonId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AuthorPersonId",
                table: "Activities",
                column: "AuthorPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Persons_AuthorPersonId",
                table: "Activities",
                column: "AuthorPersonId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
