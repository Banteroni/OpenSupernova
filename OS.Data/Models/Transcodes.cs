namespace OS.Data.Models;

public class Transcode : BaseModel
{
    public required string Path;
    public required string Format;
    public required string Bitrate;
    public required string SampleRate;
    public required string Channels;
    public required string Codec;
    public Guid TrackId;

    // Navigation properties
    public Track Track { get; }
}