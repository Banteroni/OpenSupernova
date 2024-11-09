using OS.Data.Files;

namespace OS.Data.Tests.Files;

[TestFixture]
public class FlacFileTest : IDisposable
{
    private FlacFile _flacFile;


    [SetUp]
    public void Setup()
    {
        using var stream = new FileStream("./dummy.flac", FileMode.Open, FileAccess.Read, FileShare.Read);
        _flacFile = new FlacFile(stream);
    }

    [Test]
    public void RetrieveTrackTitle()
    {
        var title = _flacFile.GetTrackTitle();
        Assert.That(title, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumArtist()
    {
        var albumArtist = _flacFile.GetAlbumArtist();
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumName()
    {
        var albumArtist = _flacFile.GetAlbumName();
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveTrackNumber()
    {
        var trackNumber = _flacFile.GetTrackNumber();
        Assert.That(trackNumber, Is.Not.Null);
    }

    [Test]
    public void RetrievePicture()
    {
        var pictures = _flacFile.GetPictures();
        Assert.That(pictures.Count(), Is.GreaterThan(0));
    }

    [Test]
    public void RetrieveNotPresentComment()
    {
        var genre = _flacFile.GetTrackPerformer();
        Assert.That(genre, Is.Null);
    }

    [Test]
    public void RetrieveNotPresentPicture()
    {
        var picture = _flacFile.GetPicture(MediaType.FileIcon32X32);
        Assert.That(picture, Is.Null);
    }

    public void Dispose()
    {
        _flacFile.Dispose();
    }
}