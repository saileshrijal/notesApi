using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notes.Data.Migrations
{
    /// <inheritdoc />
    public partial class relationshipwithcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Note",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Category",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_ApplicationUserId",
                table: "Note",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_ApplicationUserId",
                table: "Category",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_ApplicationUserId",
                table: "Category",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_AspNetUsers_ApplicationUserId",
                table: "Note",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_ApplicationUserId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_AspNetUsers_ApplicationUserId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_ApplicationUserId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Category_ApplicationUserId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Category");
        }
    }
}
