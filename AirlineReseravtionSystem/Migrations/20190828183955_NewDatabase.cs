using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirlineReseravtionSystem.Migrations
{
    public partial class NewDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightSeatings",
                columns: table => new
                {
                    FlightSeatingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FlightNumber = table.Column<int>(nullable: false),
                    FirstClassSeatNumbers = table.Column<string>(nullable: true),
                    FirstClassSeatStatus = table.Column<string>(nullable: true),
                    EconomyClassSeatNumbers = table.Column<string>(nullable: true),
                    EconomyClassSeatStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSeatings", x => x.FlightSeatingID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightSeatings");
        }
    }
}
