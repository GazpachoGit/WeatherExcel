using Microsoft.EntityFrameworkCore.Migrations;

namespace weatherApp.Migrations
{
    public partial class culumnsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Month = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Time = table.Column<string>(nullable: true),
                    Temperature = table.Column<string>(nullable: true),
                    Humidity = table.Column<string>(nullable: true),
                    DewPoint = table.Column<string>(nullable: true),
                    WindDirection = table.Column<string>(nullable: true),
                    Pressure = table.Column<string>(nullable: true),
                    WindSpeed = table.Column<string>(nullable: true),
                    Cloudy = table.Column<string>(nullable: true),
                    CloudyLowBorder = table.Column<string>(nullable: true),
                    Visibility = table.Column<string>(nullable: true),
                    WeatherPhenomenon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherInfo", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherInfo");
        }
    }
}
