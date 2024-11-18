using System.Text;
// ReSharper disable UnusedMember.Local

namespace OS.Data.Files;

public class FlacFile(byte[] data) : BaseFile(data)
{
    private enum BlockType
    {
        StreamInfo,
        Padding,
        Application,
        SeekTable,
        VorbisComment,
        CueSheet,
        Picture
    }

    public override bool IsCorrectFormat()
    {

        // Check if first 4 bytes equals "fLaC"
        var flacHeader = Data.Take(4).ToArray();
        return Encoding.UTF8.GetString(flacHeader) == "fLaC";
    }

    public override string? GetAlbumGenre()
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Genre);
    }

    public override string? GetAlbumName()
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Album);
    }

    public override int? GetAlbumYear()
    {
        var year = ExtractVorbisComment(FlacVorbisCommentField.Year);
        if (year == null)
        {
            return null;
        }
        if (year.Length > 4)
        {
            year = year.Substring(0, 4);
        }

        int.TryParse(year, out var yearInt);
        return yearInt;
    }

    public override string? GetAlbumArtist()
    {
        return ExtractVorbisComment(FlacVorbisCommentField.AlbumArtist);
    }

    public override string? GetTrackTitle()
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Title);
    }

    public override string? GetTrackArtist()
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Artist);
    }

    public override int? GetTrackNumber()
    {
        var trackNumber = ExtractVorbisComment(FlacVorbisCommentField.TrackNumber);
        if (trackNumber == null)
        {
            return null;
        }

        if (trackNumber.Contains('/'))
        {
            var trackNumberSplit = trackNumber.Split('/');
            trackNumber = trackNumberSplit[0];
        }
        int.TryParse(trackNumber, out var trackNumberInt);
        return trackNumberInt;
    }

    public override string? GetTrackPerformer()
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Performer);
    }

    public override IEnumerable<MetadataPicture> GetPictures()
    {
        var pictureBlocks = FindCorrectMetadataSection(BlockType.Picture);
        return pictureBlocks.Select(ExtractMetadataPicture);
    }

    public override MetadataPicture? GetPicture(MediaType type)
    {
        var pictureBlocks = FindCorrectMetadataSection(BlockType.Picture);
        var bytesEnumerable = pictureBlocks.ToList();
        return bytesEnumerable.Count == 0
            ? null
            : bytesEnumerable.Select(ExtractMetadataPicture)
                .FirstOrDefault(metadataPicture => metadataPicture.Type == type);
    }

    private static MetadataPicture ExtractMetadataPicture(byte[] block)
    {
        using var pictureBinaryStream = new MemoryStream(block);
        using var pictureBinaryReader = new BinaryReader(pictureBinaryStream);

        var pictureType = (MediaType)ToBigEndian(pictureBinaryReader.ReadBytes(4));
        var mimeTypeLength = ToBigEndian(pictureBinaryReader.ReadBytes(4));
        var mimeType = Encoding.UTF8.GetString(pictureBinaryReader.ReadBytes(mimeTypeLength));
        var description = Encoding.UTF8.GetString(pictureBinaryReader.ReadBytes(4));
        var width = ToBigEndian(pictureBinaryReader.ReadBytes(4));
        var height = ToBigEndian(pictureBinaryReader.ReadBytes(4));
        pictureBinaryReader.BaseStream.Seek(8, SeekOrigin.Current);
        var pictureLength = ToBigEndian(pictureBinaryReader.ReadBytes(4));
        using var data = new MemoryStream(pictureBinaryReader.ReadBytes(pictureLength));
        var dataBytes = data.ToArray();
        return new MetadataPicture(pictureType, mimeType, description, dataBytes, width, height);
    }

    private string? ExtractVorbisComment(string comment)
    {
        var metadata = FindCorrectMetadataSection(BlockType.VorbisComment);
        var bytesEnumerable = metadata.ToList();
        if (bytesEnumerable.Count == 0)
        {
            return null;
        }

        var comments = ExtractVorbisComments(bytesEnumerable.First());
        comments.TryGetValue(comment, out var value);
        return value;
    }

    private static int ToBigEndian(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }

        return BitConverter.ToInt32(data, 0);
    }

    private static Dictionary<string, string> ExtractVorbisComments(byte[] blockData)
    {
        var comments = new Dictionary<string, string>();

        var vendorStringLength = BitConverter.ToInt32(blockData, 0);

        // Get the number of comments
        var numComments = BitConverter.ToInt32(blockData, 4 + vendorStringLength);

        // Start reading comments
        var commentOffset = 8 + vendorStringLength;
        for (var i = 0; i < numComments; i++)
        {
            var commentLength = BitConverter.ToInt32(blockData, commentOffset);
            commentOffset += 4;

            var comment = Encoding.UTF8.GetString(blockData, commentOffset, commentLength);
            commentOffset += commentLength;

            // Split comment into key and value
            var keyValue = comment.Split(['='], 2);
            if (keyValue.Length == 2)
            {
                comments[keyValue[0]] = keyValue[1];
            }
        }

        return comments;
    }

    private IEnumerable<byte[]> FindCorrectMetadataSection(BlockType wantedBlockType)
    {
        List<byte[]> metadata = [];
        // Create a binary reader for the data
        using var memStream = new MemoryStream(Data);
        using var binaryReader = new BinaryReader(memStream);

        // Skip the first 4 bytes for flac validation
        binaryReader.BaseStream.Seek(4, SeekOrigin.Begin);

        var endOfMetadata = false;

        while (!endOfMetadata)
        {
            // Read the block header
            var blockHeader = binaryReader.ReadBytes(4);
            // Read if its end of metadata, so first bit of first byte is 1
            endOfMetadata = (blockHeader[0] & 0x80) == 0x80;
            // Read the block type (7 bits)
            var blockType = (BlockType)(blockHeader[0] & 0x7F);
            // Read the length of the block
            var blockLength = (uint)(blockHeader[1] << 16 | blockHeader[2] << 8 | blockHeader[3]);

            // If the block type is the one we are looking for
            if (blockType == wantedBlockType)
            {
                metadata.Add(binaryReader.ReadBytes((int)blockLength));
            }
            else
            {
                // Skip the block
                binaryReader.BaseStream.Seek(blockLength, SeekOrigin.Current);
            }
        }

        return metadata.AsEnumerable();
    }
}

internal static class FlacVorbisCommentField
{
    public const string Title = "TITLE";
    public const string Version = "VERSION";
    public const string Album = "ALBUM";
    public const string TrackNumber = "TRACKNUMBER";
    public const string Artist = "ARTIST";
    public const string Performer = "PERFORMER";
    public const string Copyright = "COPYRIGHT";
    public const string License = "LICENSE";
    public const string Organization = "ORGANIZATION";
    public const string Description = "DESCRIPTION";
    public const string Genre = "GENRE";
    public const string Year = "DATE";
    public const string Location = "LOCATION";
    public const string Contact = "CONTACT";

    public const string Isrc = "ISRC";

    // I haven't got a clue if the next ones exist in all flac files, but I'm adding them anyway
    public const string AlbumArtist = "ALBUMARTIST";
}