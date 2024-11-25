using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addFilesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoredEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Mime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredEntities_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoredEntities_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoredEntities_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoredEntities_AlbumId",
                table: "StoredEntities",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredEntities_ArtistId",
                table: "StoredEntities",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredEntities_TrackId",
                table: "StoredEntities",
                column: "TrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredEntities");
        }
    }
}
