using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialRebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Last_Name",
                table: "User_Registration_Detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approval_Status",
                table: "Service_Provider",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Service_Provider",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Blocked",
                table: "Service_Provider",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Profile_Image_Url",
                table: "Service_Provider",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rejection_Reason",
                table: "Service_Provider",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "Login_Detail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Last_Notification",
                table: "Login_Detail",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Must_Change_Password",
                table: "Login_Detail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_On",
                table: "FeedBack_Detail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "Event_Status_Master",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Event_Type",
                table: "Event_Status_Master",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Event_Status_Master",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "State_Id_fk",
                table: "City_Master",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Booking_Reference",
                table: "Booking_Cart_Detail",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Booking_Status",
                table: "Booking_Cart_Detail",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_On",
                table: "Booking_Cart_Detail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Event_Manager_Approved",
                table: "Booking_Cart_Detail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Event_Manager_Login_Id_fk",
                table: "Booking_Cart_Detail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Guest_Count",
                table: "Booking_Cart_Detail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Payment_Status",
                table: "Booking_Cart_Detail",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Payment_Detail",
                columns: table => new
                {
                    Payment_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Booking_Id_fk = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Payment_Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Payment_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Transaction_Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Paid_On = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Detail", x => x.Payment_Id);
                    table.ForeignKey(
                        name: "FK_Payment_Detail_Booking_Cart_Detail_Booking_Id_fk",
                        column: x => x.Booking_Id_fk,
                        principalTable: "Booking_Cart_Detail",
                        principalColumn: "Booking_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service_Catalog_Item",
                columns: table => new
                {
                    Service_Catalog_Item_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Service_Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Event_Type_Id_fk = table.Column<int>(type: "int", nullable: false),
                    Service_Provider_Id_fk = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Photo_Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service_Catalog_Item", x => x.Service_Catalog_Item_Id);
                    table.ForeignKey(
                        name: "FK_Service_Catalog_Item_Event_Type_Master_Event_Type_Id_fk",
                        column: x => x.Event_Type_Id_fk,
                        principalTable: "Event_Type_Master",
                        principalColumn: "Event_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Service_Catalog_Item_Service_Provider_Service_Provider_Id_fk",
                        column: x => x.Service_Provider_Id_fk,
                        principalTable: "Service_Provider",
                        principalColumn: "Service_Provider_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "State_Master",
                columns: table => new
                {
                    State_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State_Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State_Master", x => x.State_Id);
                });

            migrationBuilder.CreateTable(
                name: "Booking_Service_Detail",
                columns: table => new
                {
                    Booking_Service_Detail_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Booking_Id_fk = table.Column<int>(type: "int", nullable: false),
                    Service_Catalog_Item_Id_fk = table.Column<int>(type: "int", nullable: false),
                    Service_Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Confirmation_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Provider_Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Confirmed_On = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking_Service_Detail", x => x.Booking_Service_Detail_Id);
                    table.ForeignKey(
                        name: "FK_Booking_Service_Detail_Booking_Cart_Detail_Booking_Id_fk",
                        column: x => x.Booking_Id_fk,
                        principalTable: "Booking_Cart_Detail",
                        principalColumn: "Booking_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Service_Detail_Service_Catalog_Item_Service_Catalog_Item_Id_fk",
                        column: x => x.Service_Catalog_Item_Id_fk,
                        principalTable: "Service_Catalog_Item",
                        principalColumn: "Service_Catalog_Item_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Registration_Detail_Area_Id_fk",
                table: "User_Registration_Detail",
                column: "Area_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Provider_Service_Provider_Area_Id_fk",
                table: "Service_Provider",
                column: "Service_Provider_Area_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Status_Master_Event_Booking_Cart_fk",
                table: "Event_Status_Master",
                column: "Event_Booking_Cart_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Status_Master_Service_Person_Id_fk",
                table: "Event_Status_Master",
                column: "Service_Person_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_City_Master_State_Id_fk",
                table: "City_Master",
                column: "State_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Cart_Detail_Custom_Id_fk",
                table: "Booking_Cart_Detail",
                column: "Custom_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Cart_Detail_Event_Manager_Login_Id_fk",
                table: "Booking_Cart_Detail",
                column: "Event_Manager_Login_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Cart_Detail_Event_Package_Id_fk",
                table: "Booking_Cart_Detail",
                column: "Event_Package_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Service_Detail_Booking_Id_fk",
                table: "Booking_Service_Detail",
                column: "Booking_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Service_Detail_Service_Catalog_Item_Id_fk",
                table: "Booking_Service_Detail",
                column: "Service_Catalog_Item_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Detail_Booking_Id_fk",
                table: "Payment_Detail",
                column: "Booking_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Catalog_Item_Event_Type_Id_fk",
                table: "Service_Catalog_Item",
                column: "Event_Type_Id_fk");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Catalog_Item_Service_Provider_Id_fk",
                table: "Service_Catalog_Item",
                column: "Service_Provider_Id_fk");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Cart_Detail_Event_Type_Master_Event_Package_Id_fk",
                table: "Booking_Cart_Detail",
                column: "Event_Package_Id_fk",
                principalTable: "Event_Type_Master",
                principalColumn: "Event_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Cart_Detail_Login_Detail_Event_Manager_Login_Id_fk",
                table: "Booking_Cart_Detail",
                column: "Event_Manager_Login_Id_fk",
                principalTable: "Login_Detail",
                principalColumn: "Login_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Cart_Detail_User_Registration_Detail_Custom_Id_fk",
                table: "Booking_Cart_Detail",
                column: "Custom_Id_fk",
                principalTable: "User_Registration_Detail",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_City_Master_State_Master_State_Id_fk",
                table: "City_Master",
                column: "State_Id_fk",
                principalTable: "State_Master",
                principalColumn: "State_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Status_Master_Booking_Cart_Detail_Event_Booking_Cart_fk",
                table: "Event_Status_Master",
                column: "Event_Booking_Cart_fk",
                principalTable: "Booking_Cart_Detail",
                principalColumn: "Booking_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Status_Master_Login_Detail_Service_Person_Id_fk",
                table: "Event_Status_Master",
                column: "Service_Person_Id_fk",
                principalTable: "Login_Detail",
                principalColumn: "Login_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Provider_Area_Master_Service_Provider_Area_Id_fk",
                table: "Service_Provider",
                column: "Service_Provider_Area_Id_fk",
                principalTable: "Area_Master",
                principalColumn: "Area_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Registration_Detail_Area_Master_Area_Id_fk",
                table: "User_Registration_Detail",
                column: "Area_Id_fk",
                principalTable: "Area_Master",
                principalColumn: "Area_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Cart_Detail_Event_Type_Master_Event_Package_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Cart_Detail_Login_Detail_Event_Manager_Login_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Cart_Detail_User_Registration_Detail_Custom_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropForeignKey(
                name: "FK_City_Master_State_Master_State_Id_fk",
                table: "City_Master");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Status_Master_Booking_Cart_Detail_Event_Booking_Cart_fk",
                table: "Event_Status_Master");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Status_Master_Login_Detail_Service_Person_Id_fk",
                table: "Event_Status_Master");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Provider_Area_Master_Service_Provider_Area_Id_fk",
                table: "Service_Provider");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Registration_Detail_Area_Master_Area_Id_fk",
                table: "User_Registration_Detail");

            migrationBuilder.DropTable(
                name: "Booking_Service_Detail");

            migrationBuilder.DropTable(
                name: "Payment_Detail");

            migrationBuilder.DropTable(
                name: "State_Master");

            migrationBuilder.DropTable(
                name: "Service_Catalog_Item");

            migrationBuilder.DropIndex(
                name: "IX_User_Registration_Detail_Area_Id_fk",
                table: "User_Registration_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Service_Provider_Service_Provider_Area_Id_fk",
                table: "Service_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Event_Status_Master_Event_Booking_Cart_fk",
                table: "Event_Status_Master");

            migrationBuilder.DropIndex(
                name: "IX_Event_Status_Master_Service_Person_Id_fk",
                table: "Event_Status_Master");

            migrationBuilder.DropIndex(
                name: "IX_City_Master_State_Id_fk",
                table: "City_Master");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Cart_Detail_Custom_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Cart_Detail_Event_Manager_Login_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Cart_Detail_Event_Package_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Last_Name",
                table: "User_Registration_Detail");

            migrationBuilder.DropColumn(
                name: "Approval_Status",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Is_Blocked",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Profile_Image_Url",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Rejection_Reason",
                table: "Service_Provider");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Login_Detail");

            migrationBuilder.DropColumn(
                name: "Last_Notification",
                table: "Login_Detail");

            migrationBuilder.DropColumn(
                name: "Must_Change_Password",
                table: "Login_Detail");

            migrationBuilder.DropColumn(
                name: "Created_On",
                table: "FeedBack_Detail");

            migrationBuilder.DropColumn(
                name: "State_Id_fk",
                table: "City_Master");

            migrationBuilder.DropColumn(
                name: "Booking_Reference",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Booking_Status",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Created_On",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Event_Manager_Approved",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Event_Manager_Login_Id_fk",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Guest_Count",
                table: "Booking_Cart_Detail");

            migrationBuilder.DropColumn(
                name: "Payment_Status",
                table: "Booking_Cart_Detail");

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "Event_Status_Master",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Event_Type",
                table: "Event_Status_Master",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Event_Status_Master",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }
    }
}
