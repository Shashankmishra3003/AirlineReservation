using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirlineReseravtionSystem.Migrations
{
    public partial class reservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservationInfos",
                columns: table => new
                {
                    ReservationInfoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FlightNumber = table.Column<int>(nullable: false),
                    JourneryDate = table.Column<string>(nullable: true),
                    BookingDate = table.Column<DateTime>(nullable: false),
                    FirstNames = table.Column<string>(nullable: true),
                    LastNames = table.Column<string>(nullable: true),
                    DOBs = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationInfos", x => x.ReservationInfoID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationInfos");
        }
    }
}
