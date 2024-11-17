using OS.Data.Files;

namespace OS.Data.Tests.Files;

[TestFixture]
public class FlacFileTest : IDisposable
{
    private FlacFile _flacFile;
    private FileStream _stream;


    [SetUp]
    public void Setup()
    {
        _stream = new FileStream("./dummy.flac", FileMode.Open, FileAccess.Read, FileShare.Read);
        var buffer = new byte[_stream.Length];
        _stream.Read(buffer, 0, (int)_stream.Length);
        _flacFile = new FlacFile(buffer);
    }
    [Test]
    public void CheckCorrectFormat()
    {
        var isCorrectFormat = _flacFile.IsCorrectFormat();
        Assert.That(isCorrectFormat, Is.True);
    }

    [Test]
    public void RetrieveAlbumGenre()
    {
        var genre = _flacFile.GetAlbumGenre();
        Assert.That(genre, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumYear()
    {
        var year = _flacFile.GetAlbumYear();
        Assert.That(year, Is.Not.Null);
    }

    [Test]
    public void RetrieveTrackTitle()
    {
        var title = _flacFile.GetTrackTitle();
        Assert.That(title, Is.Not.Null);
    }

    [Test]
    public void RetrieveTwoProperties()
    {
        var title = _flacFile.GetTrackTitle();
        var artist = _flacFile.GetTrackArtist();
        Assert.That(title, Is.Not.Null);
        Assert.That(artist, Is.Not.Null);
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
        _stream.Dispose();
    }
}