using System.Collections;
using System.Text;

namespace OS.Services.Metadata;

public class FlacLocalMetadataService : ILocalMetadataService
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

    public string? RetrieveAlbumName(FileStream stream)
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Album, stream);
    }

    public string? RetrieveAlbumYear(FileStream stream)
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Year, stream);
    }

    public string? RetrieveAlbumArtist(FileStream stream)
    {
        return ExtractVorbisComment(FlacVorbisCommentField.AlbumArtist, stream);
    }

    public string? RetrieveTrackTitle(FileStream stream)
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Title, stream);
    }

    public string? RetrieveTrackArtist(FileStream stream)
    {
        return ExtractVorbisComment(FlacVorbisCommentField.Artist, stream);
    }

    public string? RetrieveTrackNumber(FileStream stream)
    {
        return ExtractVorbisComment(FlacVorbisCommentField.TrackNumber, stream);
    }

    public MetadataPicture RetrievePicture(FileStream stream)
    {
        var pictureBlock = FindCorrectMetadataSection(BlockType.Picture, stream);
        // Get first 32 bits of the picture block

        var pointer = 0;
        var pictureType = (FlacMediaType)ToBigEndian(pictureBlock[..4]);
        pointer += 4;
        var mimeTypeLength = ToBigEndian(pictureBlock[pointer..(pointer + 4)]);
        pointer += 4;
        var mimeType = Encoding.UTF8.GetString(pictureBlock, pointer, mimeTypeLength);
        pointer += mimeTypeLength;
        var description = Encoding.UTF8.GetString(pictureBlock, pointer, pointer + 4);
        pointer += 4;
        var width = ToBigEndian(pictureBlock[pointer..(pointer + 4)]);
        pointer += 4;
        var height = ToBigEndian(pictureBlock[pointer..(pointer + 4)]);
        pointer += 4;
        var colorDepth = ToBigEndian(pictureBlock[pointer..(pointer + 4)]);
        pointer += 4;
        var colorCount = ToBigEndian(pictureBlock[pointer..(pointer + 4)]);
        pointer += 4;
        var pictureLength = ToBigEndian(pictureBlock[pointer..(pointer + 4)]);
        pointer += 4;
        var picture = pictureBlock[pointer..(pointer + pictureLength)];
        return new MetadataPicture()
        {
            Type = pictureType,
            MimeType = mimeType,
            Description = description,
            Data = picture
        };
    }

    private static string? ExtractVorbisComment(string comment, FileStream stream)
    {
        var metadata = FindCorrectMetadataSection(BlockType.VorbisComment, stream);
        var comments = ExtractVorbisComments(metadata);
        return comments[comment];
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

    private static byte[] FindCorrectMetadataSection(BlockType wantedBlockType, FileStream stream)
    {
        using var binaryReader = new BinaryReader(stream);

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
                return binaryReader.ReadBytes((int)blockLength);
            }
            else
            {
                // Skip the block
                binaryReader.BaseStream.Seek(blockLength, SeekOrigin.Current);
            }
        }

        return Array.Empty<byte>();
    }
}