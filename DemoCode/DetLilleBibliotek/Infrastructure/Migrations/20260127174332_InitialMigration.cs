using System;
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Navn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medlemmer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bøger",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Forfatter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErUdlånt = table.Column<bool>(type: "bit", nullable: false),
                    MedlemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bøger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bøger_Medlemmer_MedlemId",
                        column: x => x.MedlemId,
                        principalTable: "Medlemmer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bøger_Isbn",
                table: "Bøger",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bøger_MedlemId",
                table: "Bøger",
                column: "MedlemId");
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
