using OS.Services.Metadata;

namespace OS.Services.Tests.Metadata;

[TestFixture]
public class FlacLocalMetadataServiceTest
{
    private FlacLocalMetadataService _flacLocalMetadataService;
    private FileStream _flacStream;

    [SetUp]
    public void Setup()
    {
        _flacLocalMetadataService = new FlacLocalMetadataService();
        var path = Environment.GetEnvironmentVariable("FLAC_TRACK_PATH");
        // Check if file exists
        if (!Path.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (!stream.CanRead)
        {
            throw new Exception("Can't read stream");
        }

        _flacStream = stream;
    }

    [Test]
    public void RetrieveTrackTitle()
    {
        var title = _flacLocalMetadataService.RetrieveTrackTitle(_flacStream);
        Assert.That(title, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumArtist()
    {
        var albumArtist = _flacLocalMetadataService.RetrieveAlbumArtist(_flacStream);
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumName()
    {
        var albumArtist = _flacLocalMetadataService.RetrieveAlbumName(_flacStream);
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveTrackNumber()
    {
        var trackNumber = _flacLocalMetadataService.RetrieveTrackNumber(_flacStream);
        Assert.That(trackNumber, Is.Not.Null);
    }

    [Test]
    public void RetrievePicture()
    {
        var picture = _flacLocalMetadataService.RetrievePicture(_flacStream);
        Assert.That(picture, Is.Not.Null);
    }
}