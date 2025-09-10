﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenPassVault.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameToSecretEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Secret",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Secret");
        }
    }
}
