using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaMVC.Migrations
{
    public partial class InicioAlocacaoMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InicioAlocacao",
                table: "Aloc",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InicioAlocacao",
                table: "Aloc");
        }
    }
}
