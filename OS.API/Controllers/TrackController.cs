using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Services.IRepository;
using OS.Services.Jobs;
using OS.Services.Storage;
using Quartz;

namespace OS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController(
    IRepository repository,
    ITranscodeStorageService transcodeStorageService,
    ITempStorageService tempStorageService,
    IStorageService storageService, IScheduler scheduler) : Controller
{
    private readonly IRepository _repository = repository;
    private readonly ITranscodeStorageService _transcodeStorageService = transcodeStorageService;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly IStorageService _storageService = storageService;
    private readonly IScheduler _scheduler = scheduler;

    [HttpGet]
    public async Task<IActionResult> GetTracks([FromQuery] [Optional] Guid albumId,
        [FromQuery] [Optional] string? title)
    {
        var tracks = await _repository.GetTracksAsync();
        if (albumId != Guid.Empty)
        {
            tracks = tracks.Where(t => t.AlbumId == albumId);
        }

        if (title != null)
        {
            tracks = tracks.Where(t => t.Name.Contains(title));
        }

        return Ok(tracks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrack([FromRoute] Guid id)
    {
        var track = await _repository.GetTrackAsync(id);
        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }

    [HttpGet("{id}/stream")]
    public async Task<IActionResult> GetTrackStream([FromRoute] Guid id)
    {
        var track = await _repository.GetTrackAsync(id);
        if (track == null)
        {
            return NotFound();
        }

        if (track.TranscodeObject == null)
        {
            return NotFound("Track has not been transcoded yet");
        }

        var stream = await _transcodeStorageService.GetFileAsync(track.TranscodeObject);

        if (stream == null)
        {
            return NotFound("Transcoded file not found");
        }

        return File(stream, "audio/opus");
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadTrack([FromRoute] Guid id)
    {
        var track = await _repository.GetTrackAsync(id);
        if (track == null)
        {
            return NotFound();
        }

        if (track.FileObject == null)
        {
            return NotFound("Track has not been uploaded yet");
        }

        var stream = await _storageService.GetFileAsync(track.FileObject);

        if (stream == null)
        {
            return NotFound("File not found");
        }

        return File(stream, "audio/flac", track.Name + ".flac");
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] IFormFile? file)
    {
        
        if (file == null)
        {
            return BadRequest("No file provided");
        }
        if (file.Headers.ContentType != "audio/flac" && file.Headers.ContentType != "application/zip")
        {
            return BadRequest("Invalid content type");
        }
        
        await using var stream = file.OpenReadStream();
        var tempObject = Guid.NewGuid().ToString();
        try
        {
            var operationCompleted = await _tempStorageService.SaveFileAsync(stream, tempObject);
            if (!operationCompleted)
            {
                return BadRequest("Failed to save file, view logs for more information");
            }
            
            var jobData = new JobDataMap();
            jobData.Add("fileName", tempObject);
            
            var job = JobBuilder.Create<ImportTracksJob>()
                .WithIdentity("ImportTracks", "ImportGroup")
                .UsingJobData(jobData)
                .Build();
            
            var trigger = TriggerBuilder.Create()
                .WithIdentity("ImportTracksTrigger", "ImportGroup")
                .StartNow()
                .Build();
            
            await _scheduler.ScheduleJob(job, trigger);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
 
    }
}