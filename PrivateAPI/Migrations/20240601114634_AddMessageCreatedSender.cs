using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivateAPI.Migrations
{
    public partial class AddMessageCreatedSender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sender",
                table: "Messages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender",
                table: "Messages");
        }
    }
}
