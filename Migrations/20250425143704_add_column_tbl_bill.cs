using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class add_column_tbl_bill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payer",
                table: "WaterBill",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "WaterBill",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payer",
                table: "VehicleBill",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "VehicleBill",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payer",
                table: "OtherBill",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "OtherBill",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payer",
                table: "ManagementBill",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "ManagementBill",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payer",
                table: "WaterBill");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "WaterBill");

            migrationBuilder.DropColumn(
                name: "Payer",
                table: "VehicleBill");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "VehicleBill");

            migrationBuilder.DropColumn(
                name: "Payer",
                table: "OtherBill");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "OtherBill");

            migrationBuilder.DropColumn(
                name: "Payer",
                table: "ManagementBill");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ManagementBill");
        }
    }
}
