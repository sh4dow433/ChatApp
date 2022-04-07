using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatApi.Migrations
{
    public partial class Changed_FileRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "FileLocation",
                table: "Files",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileLocation",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
