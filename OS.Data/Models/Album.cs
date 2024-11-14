﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OS.Data.Models;

public class Album : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(255)] public string? Genre { get; set; }
    public int? Year { get; set; }
    [MaxLength(255)] public string? CoverPath { get; set; }

    [NotMapped] public Guid? NavigationArtistId { get; set; }

    // Navigation properties
    public Artist? Artist { get; set; }
    public ICollection<Track> Tracks { get; set; } = [];
}