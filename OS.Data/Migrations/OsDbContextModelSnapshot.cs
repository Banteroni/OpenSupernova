﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OS.Data.Context;

#nullable disable

namespace OS.Data.Migrations
{
    [DbContext(typeof(OsDbContext))]
    partial class OsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OS.Data.Models.Album", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CoverPath")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Genre")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("Year")
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

                    b.Property<string>("ImagePath")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Artists");

                    b.HasData(
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000001"),
                            Name = "Unknown"
                        });
                });

            modelBuilder.Entity("OS.Data.Models.Track", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AlbumId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ArtistId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("OS.Data.Models.Album", b =>
                {
                    b.HasOne("OS.Data.Models.Artist", "Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId");

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("OS.Data.Models.Track", b =>
                {
                    b.HasOne("OS.Data.Models.Album", "Album")
                        .WithMany("Tracks")
                        .HasForeignKey("AlbumId");

                    b.HasOne("OS.Data.Models.Artist", null)
                        .WithMany("Tracks")
                        .HasForeignKey("ArtistId");

                    b.Navigation("Album");
                });

            modelBuilder.Entity("OS.Data.Models.Album", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("OS.Data.Models.Artist", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("Tracks");
                });
#pragma warning restore 612, 618
        }
    }
}