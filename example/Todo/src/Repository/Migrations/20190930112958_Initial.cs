using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    FaIcon = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NoteId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NoteId);
                    table.ForeignKey(
                        name: "FK_Notes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteItem",
                columns: table => new
                {
                    NoteItemId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoteId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Completed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteItem", x => x.NoteItemId);
                    table.ForeignKey(
                        name: "FK_NoteItem_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "NoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Color", "Deleted", "FaIcon", "Title" },
                values: new object[] { 1L, "D7AEFB", false, "fa-tasks", "Tasks" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Color", "Deleted", "FaIcon", "Title" },
                values: new object[] { 2L, "FFF475", false, "fa-shopping-cart", "Shopping list" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Color", "Deleted", "FaIcon", "Title" },
                values: new object[] { 3L, "CCFF90", false, "fa-parachute-box", "Supplies" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Color", "Deleted", "FaIcon", "Title" },
                values: new object[] { 4L, "A7FFEB", false, "fa-film", "Movies" });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "NoteId", "CategoryId", "Title" },
                values: new object[] { 3L, 1L, "Miscellaneous" });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "NoteId", "CategoryId", "Title" },
                values: new object[] { 1L, 2L, "Shopping List" });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "NoteId", "CategoryId", "Title" },
                values: new object[] { 2L, 4L, "Movies To See" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 7L, false, 3L, "Build time machine" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 8L, false, 3L, "Grow epic beard" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 9L, false, 3L, "Plant lettuce" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 10L, false, 3L, "World domination" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 1L, false, 1L, "Bread" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 2L, false, 1L, "Lotus Biscoff Spread" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 3L, false, 1L, "Suger Cubes" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 4L, false, 1L, "Water" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 5L, false, 2L, "Ad Astra" });

            migrationBuilder.InsertData(
                table: "NoteItem",
                columns: new[] { "NoteItemId", "Completed", "NoteId", "Title" },
                values: new object[] { 6L, false, 2L, "It 2" });

            migrationBuilder.CreateIndex(
                name: "IX_NoteItem_NoteId",
                table: "NoteItem",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CategoryId",
                table: "Notes",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoteItem");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
