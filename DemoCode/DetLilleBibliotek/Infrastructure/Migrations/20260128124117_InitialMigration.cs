using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medlemmer",
                columns: table => new
                {
                    Medlemsnummer = table.Column<int>(type: "int", nullable: false),
                    Navn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medlemmer", x => x.Medlemsnummer);
                });

            migrationBuilder.CreateTable(
                name: "Bøger",
                columns: table => new
                {
                    Isbn = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Forfatter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErUdlånt = table.Column<bool>(type: "bit", nullable: false),
                    Medlemsnummer = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bøger", x => x.Isbn);
                    table.ForeignKey(
                        name: "FK_Bøger_Medlemmer_Medlemsnummer",
                        column: x => x.Medlemsnummer,
                        principalTable: "Medlemmer",
                        principalColumn: "Medlemsnummer");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bøger_Medlemsnummer",
                table: "Bøger",
                column: "Medlemsnummer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bøger");

            migrationBuilder.DropTable(
                name: "Medlemmer");
        }
    }
}
