using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPic.Migrations.BannedProfile
{
    public partial class BannedProfileMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BannedProfiles",
                columns: table => new
                {
                    BannedId = table.Column<string>(type: "int", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    BanDatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedProfiles", x => x.BannedId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannedProfiles");
        }
    }
}
