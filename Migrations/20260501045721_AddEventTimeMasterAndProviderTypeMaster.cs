using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTimeMasterAndProviderTypeMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event_Time_Master",
                columns: table => new
                {
                    Event_Time_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event_Time_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event_Time_Master", x => x.Event_Time_Id);
                });

            migrationBuilder.CreateTable(
                name: "Service_Provider_Type_Master",
                columns: table => new
                {
                    Provider_Type_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service_Provider_Type_Master", x => x.Provider_Type_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event_Time_Master");

            migrationBuilder.DropTable(
                name: "Service_Provider_Type_Master");
        }
    }
}
