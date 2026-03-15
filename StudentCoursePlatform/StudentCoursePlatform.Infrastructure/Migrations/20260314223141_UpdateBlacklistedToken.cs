using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentCoursePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlacklistedToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedTokens",
                table: "BlacklistedTokens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BlacklistedTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedTokens",
                table: "BlacklistedTokens",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedTokens",
                table: "BlacklistedTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "BlacklistedTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedTokens",
                table: "BlacklistedTokens",
                column: "Id");
        }
    }
}
