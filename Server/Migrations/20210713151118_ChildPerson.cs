using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class ChildPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildPerson");

            migrationBuilder.CreateTable(
                name: "ChildPersons",
                columns: table => new
                {
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildPersons", x => new { x.ChildId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_ChildPersons_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "ChildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildPersons_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChildPersons_PersonId",
                table: "ChildPersons",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildPersons");

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
                name: "IX_ChildPerson_ChildrenChildId",
                table: "ChildPerson",
                column: "ChildrenChildId");
        }
    }
}
