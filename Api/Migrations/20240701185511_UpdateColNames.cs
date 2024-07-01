using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nombres",
                table: "AspNetUsers",
                newName: "PaternalLastName");

            migrationBuilder.RenameColumn(
                name: "ApellidoPaterno",
                table: "AspNetUsers",
                newName: "Names");

            migrationBuilder.RenameColumn(
                name: "ApellidoMaterno",
                table: "AspNetUsers",
                newName: "MothersLastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaternalLastName",
                table: "AspNetUsers",
                newName: "Nombres");

            migrationBuilder.RenameColumn(
                name: "Names",
                table: "AspNetUsers",
                newName: "ApellidoPaterno");

            migrationBuilder.RenameColumn(
                name: "MothersLastName",
                table: "AspNetUsers",
                newName: "ApellidoMaterno");
        }
    }
}
