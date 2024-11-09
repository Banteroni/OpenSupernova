using OS.Data.Files;

namespace OS.Data.Tests.Files;

[TestFixture]
public class FlacFileTest : IDisposable
{

    private FlacFile _flacFile;
    

    [SetUp]
    public void Setup()
    {
        var path = Environment.GetEnvironmentVariable("FLAC_TRACK_PATH");
        if (!Path.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (!stream.CanRead)
        {
            throw new Exception("Can't read stream");
        }

        _flacFile = new FlacFile(stream);

    }

    [Test]
    public void RetrieveTrackTitle()
    {
        var title = _flacFile.RetrieveTrackTitle();
        Assert.That(title, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumArtist()
    {
        var albumArtist = _flacFile.RetrieveAlbumArtist();
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumName()
    {
        var albumArtist = _flacFile.RetrieveAlbumName();
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveTrackNumber()
    {
        var trackNumber = _flacFile.RetrieveTrackNumber();
        Assert.That(trackNumber, Is.Not.Null);
    }

    [Test]
    public void RetrievePicture()
    {
        var pictures = _flacFile.RetrievePictures();
        Assert.That(pictures.Count(), Is.GreaterThan(0));
    }

    public void Dispose()
    {
        _flacFile.Dispose();
    }
}