using Microsoft.Extensions.Logging;
using Moq;
using OS.Data.Files;
using OS.Data.Models;
using OS.Data.Options;
using OS.Data.Repository.Conditions;
using OS.Services.Codec;
using OS.Services.Jobs;
using OS.Services.Repository;
using OS.Services.Storage;
using Quartz;
using System.IO;
using System.IO.Pipes;
using System.Text;
using static OS.Data.Files.FlacFile;
using MockRepository = OS.Services.Repository.MockRepository;

namespace OS.Services.Tests.Jobs;

public class ImportTracksJobTest
{
    private Mock<ILogger<ImportTracksJob>> _jobLogger;
    private Mock<IRepository> _repository;
    private Mock<IStorageService> _storageService;
    private Mock<ITempStorageService> _tempStorageService;
    private Mock<ITranscoder> _transcoder;
    private ImportTracksJob _job;


    [SetUp]
    public async Task Setup()
    {
        _jobLogger = new Mock<ILogger<ImportTracksJob>>();
        _repository = new Mock<IRepository>();
        _storageService = new Mock<IStorageService>();
        _tempStorageService = new Mock<ITempStorageService>();
        _transcoder = new Mock<ITranscoder>();

        _job = new ImportTracksJob(_jobLogger.Object, _storageService.Object, _tempStorageService.Object, _transcoder.Object, _repository.Object);
    }

    [Test]
    public async Task FileDoesNotExist()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "empty.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);
        _tempStorageService.Setup(x => x.FileExistsAsync("empty.flac")).ReturnsAsync(false);

        //Act
        await _job.Execute(context.Object);

        //Assert
        _jobLogger.Verify(
    m => m.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("does not exist")),
        It.IsAny<Exception>(),
        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
    Times.Once);
    }

    [Test]
    public async Task FileNameNotFound()
    {
        // Arrange
        var jobData = new JobDataMap();
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        //Act
        await _job.Execute(context.Object);

        //Assert
        _jobLogger.Verify(
    m => m.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("File path not found in the job data")),
        It.IsAny<Exception>(),
        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
    Times.Once);
    }

    [Test]
    public async Task FileNameEmpty()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        //Act
        await _job.Execute(context.Object);

        //Assert
        _jobLogger.Verify(
            m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("File path is empty")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Test]
    public async Task ImportTracksWithZip()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "dummy.zip");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);
        _tempStorageService.Setup(x => x.IsFileZip(It.IsAny<string>())).ReturnsAsync(true);
        _tempStorageService.Setup(x => x.ExtractZipAsync(It.IsAny<string>())).ReturnsAsync(new List<string> { "dummy.flac" });
        _tempStorageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        // read stream from dummy.zip
        using var stream = File.OpenRead("dummy.flac");
        _tempStorageService.Setup(x => x.GetFileStream(It.IsAny<string>())).ReturnsAsync(stream);
        _transcoder.Setup(x => x.TranscodeAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        var artist = new Artist { Id = Guid.NewGuid(), Name = "Artist" };
        _repository.Setup(x => x.GetListAsync<Artist>(It.IsAny<SimpleCondition>(), It.IsAny<string[]>())).ReturnsAsync(new List<Artist>([artist]));
        var album = new Album { Id = Guid.NewGuid(), Name = "Album", Artist = artist };
        _repository.Setup(x => x.GetListAsync<Album>(It.IsAny<CompositeCondition>(), It.IsAny<string[]>())).ReturnsAsync(new List<Album>([album]));
        _repository.Setup(x => x.CreateAsync<Track>(It.IsAny<Track>())).ReturnsAsync(new Track { Id = Guid.NewGuid(), Name = "dummy" });
        _repository.Setup(x => x.GetListAsync<Track>(It.IsAny<string[]>())).ReturnsAsync(new List<Track>());

        // Act
        await _job.Execute(context.Object);

        // Assert
        VerifyMessageWasCalled("added to the database");

    }

    [Test]
    public async Task ImportTracksWithNullStream()
    {
        var jobData = new JobDataMap();
        jobData.Add("fileName", "sette.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);
        _tempStorageService.Setup(x => x.IsFileZip(It.IsAny<string>())).ReturnsAsync(false);
        _tempStorageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _tempStorageService.Setup(x => x.GetFileStream(It.IsAny<string>())).ReturnsAsync(new MemoryStream());

        // Act
        await _job.Execute(context.Object);

        // Assert
        VerifyMessageWasCalled("Failed to get file", LogLevel.Error);
    }

    [Test]
    public async Task ImportTracksWithArtistNameNull()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "dummy.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        using var stream = File.OpenRead("dummy.flac");
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        ManipulateVorbisComment(FlacVorbisCommentField.Artist, memoryStream, "");
        memoryStream.Position = 0;


        _tempStorageService.Setup(x => x.IsFileZip(It.IsAny<string>())).ReturnsAsync(false);
        _tempStorageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _tempStorageService.Setup(x => x.GetFileStream(It.IsAny<string>())).ReturnsAsync(memoryStream);
        _repository.Setup(x => x.GetAsync<Artist>(It.IsAny<Guid>(), It.IsAny<string[]>())).ReturnsAsync(new Artist { Id = Guid.NewGuid(), Name = "Unknown" });
        await _job.Execute(context.Object);
    }

    [Test]
    public async Task ImportTracksWithTitleNameNull()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "dummy.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        using var stream = File.OpenRead("dummy.flac");
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        ManipulateVorbisComment(FlacVorbisCommentField.Title, memoryStream, "");


        _tempStorageService.Setup(x => x.IsFileZip(It.IsAny<string>())).ReturnsAsync(false);
        _tempStorageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _tempStorageService.Setup(x => x.GetFileStream(It.IsAny<string>())).ReturnsAsync(memoryStream);
        await _job.Execute(context.Object);

        VerifyMessageWasCalled("Failed to get title from file", LogLevel.Error);
    }


    // Private methods
    private void VerifyMessageWasCalled(string message, LogLevel logLevel = LogLevel.Information)
    {
        _jobLogger.Verify(
            m => m.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains(message)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

    // File manipulation, damn me for not using a static class for flac manipulation
    private static void ManipulateVorbisComment(string comment, Stream stream, string newValue)
    {
        var metadata = FindCorrectMetadataSection(BlockType.VorbisComment, stream);
        var bytesEnumerable = metadata.Item1.ToList();

        var comments = ExtractVorbisComments(bytesEnumerable.First());
        comments.TryGetValue(comment, out var value);
        var length = value.Item2;
        var valueBytes = new byte[length];
        Encoding.UTF8.GetBytes(newValue).CopyTo(valueBytes, 0);
        var position = value.Item1 + metadata.Item2;
        stream.Position = position;
        stream.Write(valueBytes, 0, valueBytes.Length);
        stream.Position = 0;


    }
    private static Dictionary<string, Tuple<int, int>> ExtractVorbisComments(byte[] blockData)
    {
        var comments = new Dictionary<string, Tuple<int, int>>();

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
                // the int is the position of the comment within the bytes array
                var position = commentOffset - commentLength;
                var length = commentLength;
                comments[keyValue[0]] = new Tuple<int, int>(position, length);
            }
        }

        return comments;
    }
    private static Tuple<IEnumerable<byte[]>, int> FindCorrectMetadataSection(BlockType wantedBlockType, Stream stream)
    {
        List<byte[]> metadata = [];
        // Create a binary reader for the data
        using (var memStream = new MemoryStream())
        {
            stream.Position = 0;
            stream.CopyTo(memStream);
            int position = 0;
            using (var binaryReader = new BinaryReader(memStream))
            {
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
                        position = (int)binaryReader.BaseStream.Position;
                        metadata.Add(binaryReader.ReadBytes((int)blockLength));
                    }
                    else
                    {
                        // Skip the block
                        binaryReader.BaseStream.Seek(blockLength, SeekOrigin.Current);
                    }
                }
                return new Tuple<IEnumerable<byte[]>, int>(metadata.AsEnumerable(), position);
            }
        }
    }
}