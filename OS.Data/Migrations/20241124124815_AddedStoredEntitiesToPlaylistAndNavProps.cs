using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStoredEntitiesToPlaylistAndNavProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlaylistId",
                table: "StoredEntities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredEntities_PlaylistId",
                table: "StoredEntities",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredEntities_Playlists_PlaylistId",
                table: "StoredEntities",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredEntities_Playlists_PlaylistId",
                table: "StoredEntities");

            migrationBuilder.DropIndex(
                name: "IX_StoredEntities_PlaylistId",
                table: "StoredEntities");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "StoredEntities");
        }
    }
}
