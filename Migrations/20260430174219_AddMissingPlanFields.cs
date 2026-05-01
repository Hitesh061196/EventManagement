using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingPlanFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Middle_Name",
                table: "User_Registration_Detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Service_Provider",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile_No",
                table: "Service_Provider",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Shop_Name",
                table: "Service_Provider",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Price_Type",
                table: "Service_Catalog_Item",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Event_Time",
                table: "Booking_Cart_Detail",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "From_Time",
                table: "Booking_Cart_Detail",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "To_Time",
                table: "Booking_Cart_Detail",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Middle_Name",
                table: "User_Registration_Detail");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Mobile_No",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Shop_Name",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Price_Type",
                table: "Service_Catalog_Item");

            migrationBuilder.DropColumn(
                name: "Event_Time",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "From_Time",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "To_Time",
                table: "Booking_Cart_Detail");
        }
    }
}
