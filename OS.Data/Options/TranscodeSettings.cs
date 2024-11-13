namespace OS.Data.Options;

public class TranscodeSettings
{
    public int BitrateKbps { get; set; }

    public string? Vbr { get; set; }
    public int SampleRate { get; set; }

    public int Channels { get; set; }

    public int CompressionLevel { get; set; }

    public string Application = "audio";

    public string Codec = "libopus";

    public string Format = "opus";
}