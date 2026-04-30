using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixEventStatusCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Status_Master_Login_Detail_Service_Person_Id_fk",
                table: "Event_Status_Master");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Status_Master_Login_Detail_Service_Person_Id_fk",
                table: "Event_Status_Master",
                column: "Service_Person_Id_fk",
                principalTable: "Login_Detail",
                principalColumn: "Login_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Status_Master_Login_Detail_Service_Person_Id_fk",
                table: "Event_Status_Master");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Status_Master_Login_Detail_Service_Person_Id_fk",
                table: "Event_Status_Master",
                column: "Service_Person_Id_fk",
                principalTable: "Login_Detail",
                principalColumn: "Login_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
