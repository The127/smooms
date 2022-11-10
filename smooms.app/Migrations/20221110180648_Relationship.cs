using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smooms.app.Migrations
{
    /// <inheritdoc />
    public partial class Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "device",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    accountid = table.Column<Guid>(name: "account_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_account_account_temp_id",
                        column: x => x.accountid,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_account_id",
                table: "device",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device");
        }
    }
}
