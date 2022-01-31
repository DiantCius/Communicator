using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class updatePerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Chats");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Chats");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
