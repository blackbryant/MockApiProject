﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mockAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ISBN = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

             
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
