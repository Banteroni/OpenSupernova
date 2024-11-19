using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Models;
using OS.Services.Repository;
using OS.Services.Jobs;
using OS.Services.Storage;
using Quartz;

namespace OS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController(
    IRepository repository,
    ITempStorageService tempStorageService,
    IStorageService storageService,
    ISchedulerFactory schedulerFactory) : Controller
{
    private readonly IRepository _repository = repository;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly IStorageService _storageService = storageService;
    private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;

    [HttpGet]
    public async Task<IActionResult> GetTracks([FromQuery][Optional] Guid albumId,
        [FromQuery][Optional] string? title)
    {
        var tracks = await _repository.GetListAsync<Track>();
        if (albumId != Guid.Empty)
        {
            tracks = tracks.Where(t => t.NavigationAlbumId == albumId);
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
        var track = await _repository.GetAsync<Track>(id);
        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }

    [HttpGet("{id}/stream")]
    public async Task<IActionResult> GetTrackStream([FromRoute] Guid id)
    {
        var track = await _repository.GetAsync<Track>(id);
        if (track == null)
        {
            return NotFound();
        }

        var stream = await _storageService.GetFileAsync(track.Id.ToString() + ".opus");

        if (stream.Length == 0)
        {
            return NotFound("Transcoded file not found");
        }

        return File(stream, "audio/opus");
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> Upload([FromForm] string description, [FromForm] DateTime clientDate, IFormFile? file)
    {
        if (file == null)
        {
            return BadRequest("No file provided");
        }

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var buffer = memoryStream.ToArray();
        try
        {
            var operationCompleted = await _tempStorageService.SaveFileAsync(buffer, file.Name);
            if (!operationCompleted)
            {
                return BadRequest("Failed to save file, view logs for more information");
            }

            var jobData = new JobDataMap { { "fileName", file.Name } };

            var job = JobBuilder.Create<ImportTracksJob>()
                .WithIdentity(Guid.NewGuid().ToString(), "ImportGroup")
                .UsingJobData(jobData)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("ImportTracksTrigger", "ImportGroup")
                .StartNow()
                .Build();

            var scheduler = await _schedulerFactory.GetScheduler();
            if (!scheduler.IsStarted)
            {
                await scheduler.Start();
            }
            await scheduler.ScheduleJob(job, trigger);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}