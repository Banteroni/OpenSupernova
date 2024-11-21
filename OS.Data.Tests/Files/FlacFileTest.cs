using Moq;
using OS.Data.Files;
using Moq.Protected;
using static OS.Data.Files.FlacFile;

namespace OS.Data.Tests.Files;

[TestFixture]
public class FlacFileTest : IDisposable
{
    private Mock<FlacFile> _flacFile;
    private FileStream _stream;


    [SetUp]
    public void Setup()
    {
        _stream = new FileStream("./dummy.flac", FileMode.Open, FileAccess.Read, FileShare.Read);
        _flacFile = new Mock<FlacFile>(_stream);
        _flacFile.CallBase = true;
    }
    [Test]
    public void CheckCorrectFormat()
    {
        var isCorrectFormat = _flacFile.Object.IsCorrectFormat();
        Assert.That(isCorrectFormat, Is.True);
    }

    [Test]
    public void RetrieveAlbumGenre()
    {
        var genre = _flacFile.Object.GetAlbumGenre();
        Assert.That(genre, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumYear()
    {
        var year = _flacFile.Object.GetAlbumYear();
        Assert.That(year, Is.Not.Null);
    }
    [Test]
    public void GetAlbumYearYYYYMMDDWithMinus()
    {
        _flacFile.Protected().Setup<string>("ExtractVorbisComment", "DATE").Returns("2000-10-29");
        var year = _flacFile.Object.GetAlbumYear();
        Assert.That(year, Is.EqualTo(2000));
    }

    [Test]
    public void GetAlbumYearYYYYMMDDWithSlash()
    {
        _flacFile.Protected().Setup<string>("ExtractVorbisComment", "DATE").Returns("2000/10/29");
        var year = _flacFile.Object.GetAlbumYear();
        Assert.That(year, Is.EqualTo(2000));
    }

    [Test]
    public void GetAlbumYearMMDDYYYYWithSlash()
    {
        _flacFile.Protected().Setup<string>("ExtractVorbisComment", "DATE").Returns("10/29/2000");
        var year = _flacFile.Object.GetAlbumYear();
        Assert.That(year, Is.EqualTo(2000));
    }

    [Test]
    public void GetAlbumYearWithNullMetadata()
    {
        _flacFile.Protected().Setup<string>("ExtractVorbisComment", "DATE").Returns((string)null);
        var year = _flacFile.Object.GetAlbumYear();
        Assert.That(year, Is.Null);
    }

    [Test]
    public void RetrieveTrackTitle()
    {
        var title = _flacFile.Object.GetTrackTitle();
        Assert.That(title, Is.Not.Null);
    }

    [Test]
    public void RetrieveTwoProperties()
    {
        var title = _flacFile.Object.GetTrackTitle();
        var artist = _flacFile.Object.GetTrackArtist();
        Assert.That(title, Is.Not.Null);
        Assert.That(artist, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumArtist()
    {
        var albumArtist = _flacFile.Object.GetAlbumArtist();
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveAlbumName()
    {
        var albumArtist = _flacFile.Object.GetAlbumName();
        Assert.That(albumArtist, Is.Not.Null);
    }

    [Test]
    public void RetrieveTrackNumber()
    {
        var trackNumber = _flacFile.Object.GetTrackNumber();
        Assert.That(trackNumber, Is.Not.Null);
    }

    [Test]
    public void RetrieveTrackNumberWithSlash()
    {
        _flacFile.Protected().Setup<string>("ExtractVorbisComment", "TRACKNUMBER").Returns("1/10");
        var trackNumber = _flacFile.Object.GetTrackNumber();
        Assert.That(trackNumber, Is.EqualTo(1));
    }

    [Test]
    public void RetrieveTrackNumberWithoutVorbisComment()
    {
        _flacFile.Protected().Setup<string>("ExtractVorbisComment", "TRACKNUMBER").Returns((string)null);
        var trackNumber = _flacFile.Object.GetTrackNumber();
        Assert.That(trackNumber, Is.Null);
    }

    [Test]
    public void RetrievePicture()
    {
        var pictures = _flacFile.Object.GetPictures();
        Assert.That(pictures.Count(), Is.GreaterThan(0));
    }

    [Test]
    public void RetrieveNotPresentComment()
    {
        var genre = _flacFile.Object.GetTrackPerformer();
        Assert.That(genre, Is.Null);
    }

    [Test]
    public void RetrieveNotPresentPicture()
    {
        var picture = _flacFile.Object.GetPicture(MediaType.FileIcon32X32);
        Assert.That(picture, Is.Null);
    }

    [Test]
    public void ExtractVorbisCommentEmptyArr()
    {
        // overrride FindCorrectMetadataSection
        _flacFile.Protected().Setup<IEnumerable<byte[]>>("FindCorrectMetadataSection", BlockType.VorbisComment).Returns(new List<byte[]>());
        var genre = _flacFile.Object.GetAlbumGenre();
        Assert.That(genre, Is.Null);
    }

    public void Dispose()
    {
        _stream.Dispose();
    }
}