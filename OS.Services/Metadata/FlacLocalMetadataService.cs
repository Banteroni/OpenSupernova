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

    public Task<string> RetrieveAlbumName(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public Task<string> RetrieveArtistName(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public async Task<string> RetrieveTrackTitle(FileStream stream)
    {
        var metadata = FindCorrectMetadataSection(BlockType.VorbisComment, stream);
        return "ok";
    }

    public Task<string> RetrieveTrackArtist(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public Task<int> RetrieveTrackNumber(FileStream stream)
    {
        throw new NotImplementedException();
    }

    private static byte[] FindCorrectMetadataSection(BlockType wantedBlockType, FileStream stream)
    {
        using var binaryReader = new BinaryReader(stream);

        // Skip the first 8 bytes, 4 are used to stream marker and 4 for STREAMINFO
        binaryReader.BaseStream.Seek(4, SeekOrigin.Begin);
        var stillInMetadataSections = true;

        while (stillInMetadataSections)
        {
            var metadataBlock = binaryReader.ReadBytes(4);

            // First chunk is used to determine if it's the last metadata block (first bit), and what blockType it is
            var firstChunk = new BitArray(new byte[1] { metadataBlock[0] });
            if (firstChunk[0] == true) stillInMetadataSections = false;

            var foundBlockType = new int[1];
            firstChunk.RightShift(1);
            firstChunk.CopyTo(foundBlockType, 0);

            // Following metadata bytes that the following block will take, before eventually having another metadata block
            var followingMetadataLength = new int[1];
            var followingMetadataBytes = metadataBlock.Skip(1).Take(3).ToArray();
            var metadataBits = new BitArray(new byte[3]
                { followingMetadataBytes[2], followingMetadataBytes[1], followingMetadataBytes[0] });
            metadataBits.CopyTo(followingMetadataLength, 0);

            // Logic to return wanted block type, or otherwise skip to next block type
            if (!Enum.IsDefined(typeof(BlockType), foundBlockType[0]) ||
                (BlockType)foundBlockType[0] != wantedBlockType)
            {
                if (stillInMetadataSections)
                    binaryReader.BaseStream.Seek(followingMetadataLength[0], SeekOrigin.Current);
            }
            else
            {
                var metadata = binaryReader.ReadBytes(followingMetadataLength[0]);
                return metadata;
            }
        }

        return Array.Empty<byte>();
    }
}