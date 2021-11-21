using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class updatePersonInvite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvited",
                table: "Persons");

            migrationBuilder.AddColumn<int>(
                name: "InvitedBy",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitedBy",
                table: "Persons");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvited",
                table: "Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
