using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OS.Data.Migrations
{
    /// <inheritdoc />
    public partial class starredSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackUser",
                columns: table => new
                {
                    StarredById = table.Column<Guid>(type: "uuid", nullable: false),
                    StarsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackUser", x => new { x.StarredById, x.StarsId });
                    table.ForeignKey(
                        name: "FK_TrackUser_Tracks_StarsId",
                        column: x => x.StarsId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackUser_Users_StarredById",
                        column: x => x.StarredById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackUser_StarsId",
                table: "TrackUser",
                column: "StarsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackUser");
        }
    }
}
