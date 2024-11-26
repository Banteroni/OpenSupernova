﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OS.Data.Context;

#nullable disable

namespace OS.Data.Migrations
{
    [DbContext(typeof(OsDbContext))]
    [Migration("20241124105848_addFilesModel")]
    partial class addFilesModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ArtistTrack", b =>
                {
                    b.Property<Guid>("ArtistsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TracksId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ArtistsId", "TracksId");

                    b.HasIndex("TracksId");

                    b.ToTable("ArtistTrack");
                });

            modelBuilder.Entity("OS.Data.Models.Album", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Genre")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("OS.Data.Models.Artist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Bio")
                        .HasMaxLength(3096)
                        .HasColumnType("nvarchar(3096)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Artists");

                    b.HasData(
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000001"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Unknown",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("OS.Data.Models.Playlist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("OS.Data.Models.StoredEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AlbumId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Mime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TrackId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ArtistId");

                    b.HasIndex("TrackId");

                    b.ToTable("StoredEntities");
                });

            modelBuilder.Entity("OS.Data.Models.Track", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AlbumId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("PlaylistTrack", b =>
                {
                    b.Property<Guid>("PlaylistsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TracksId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PlaylistsId", "TracksId");

                    b.HasIndex("TracksId");

                    b.ToTable("PlaylistTrack");
                });

            modelBuilder.Entity("ArtistTrack", b =>
                {
                    b.HasOne("OS.Data.Models.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OS.Data.Models.Track", null)
                        .WithMany()
                        .HasForeignKey("TracksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OS.Data.Models.Album", b =>
                {
                    b.HasOne("OS.Data.Models.Artist", "Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("OS.Data.Models.StoredEntity", b =>
                {
                    b.HasOne("OS.Data.Models.Album", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId");

                    b.HasOne("OS.Data.Models.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId");

                    b.HasOne("OS.Data.Models.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId");

                    b.Navigation("Album");

                    b.Navigation("Artist");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("OS.Data.Models.Track", b =>
                {
                    b.HasOne("OS.Data.Models.Album", "Album")
                        .WithMany("Tracks")
                        .HasForeignKey("AlbumId");

                    b.Navigation("Album");
                });

            modelBuilder.Entity("PlaylistTrack", b =>
                {
                    b.HasOne("OS.Data.Models.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OS.Data.Models.Track", null)
                        .WithMany()
                        .HasForeignKey("TracksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OS.Data.Models.Album", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("OS.Data.Models.Artist", b =>
                {
                    b.Navigation("Albums");
                });
#pragma warning restore 612, 618
        }
    }
}
