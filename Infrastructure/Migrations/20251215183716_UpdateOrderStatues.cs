using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderStatues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_Status_Valid",
                table: "Orders");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_Status_Valid",
                table: "Orders",
                sql: "[Status] IN ('Pending', 'PaymentReceived', 'PaymentFailed', 'PaymentMismatch')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_Status_Valid",
                table: "Orders");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_Status_Valid",
                table: "Orders",
                sql: "[Status] IN ('Pending', 'PaymentReceived', 'PaymentFailed')");
        }
    }
}
