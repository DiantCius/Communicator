using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class AddedRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Children_Persons_ParentId",
                table: "Children");

            migrationBuilder.DropIndex(
                name: "IX_Children_ParentId",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Children");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Persons",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Children",
                newName: "ChildId");

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_Activities_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "ChildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Persons_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Persons",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChildPerson",
                columns: table => new
                {
                    BabysittersPersonId = table.Column<int>(type: "int", nullable: false),
                    ChildrenChildId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildPerson", x => new { x.BabysittersPersonId, x.ChildrenChildId });
                    table.ForeignKey(
                        name: "FK_ChildPerson_Children_ChildrenChildId",
                        column: x => x.ChildrenChildId,
                        principalTable: "Children",
                        principalColumn: "ChildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildPerson_Persons_BabysittersPersonId",
                        column: x => x.BabysittersPersonId,
                        principalTable: "Persons",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AuthorId",
                table: "Activities",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ChildId",
                table: "Activities",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildPerson_ChildrenChildId",
                table: "ChildPerson",
                column: "ChildrenChildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "ChildPerson");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "Persons",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ChildId",
                table: "Children",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Children",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Children_ParentId",
                table: "Children",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Children_Persons_ParentId",
                table: "Children",
                column: "ParentId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
