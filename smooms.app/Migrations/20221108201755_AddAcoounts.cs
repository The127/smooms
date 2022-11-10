using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smooms.app.Migrations
{
    /// <inheritdoc />
    public partial class AddAcoounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    displayname = table.Column<string>(name: "display_name", type: "character varying(1000)", maxLength: 1000, nullable: false),
                    email = table.Column<string>(name: "e_mail", type: "character varying(320)", maxLength: 320, nullable: true),
                    emailverified = table.Column<bool>(name: "e_mail_verified", type: "boolean", nullable: false),
                    hashedpassword = table.Column<byte[]>(name: "hashed_password", type: "bytea", nullable: false),
                    salt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account", x => x.id);
                    table.CheckConstraint("CK_account_e_mail_EmailAddress", "e_mail ~ '^[^@]+@[^@]+$'");
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_e_mail",
                table: "account",
                column: "e_mail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
