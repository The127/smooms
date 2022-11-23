using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace smooms.api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(name: "user_name", type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    hashedpassword = table.Column<byte[]>(name: "hashed_password", type: "bytea", nullable: false),
                    salt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.CheckConstraint("CK_user_email_Length", "LENGTH(\"email\") <= 320");
                    table.CheckConstraint("CK_user_email_Regex", "\"email\" ~ '^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,5})+)$'");
                    table.CheckConstraint("CK_user_user_name_Length", "LENGTH(\"user_name\") <= 1000");
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    createdat = table.Column<Instant>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    lastaccessedat = table.Column<Instant>(name: "last_accessed_at", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_session_user_user_temp_id",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_session_user_id",
                table: "session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                table: "user",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
