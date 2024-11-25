using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Services.Repository;
using Quartz;
using OS.API.Extensions;
using OS.Services.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(x =>
{
    x.ClearProviders();
    x.AddConsole();
});

// Storage
builder.Services.AddStorage();

// Quartz
builder.Services.AddQuartz(q =>
{
    q.AddJob<ImportTracksJob>(j => j.StoreDurably().WithIdentity(ImportTracksJob.Key));

    q.AddJob<TemporaryStorageCleanupJob>(j => j.WithIdentity(TemporaryStorageCleanupJob.Key));

    q.AddJob<StorageCleanupJob>(j => j.WithIdentity(StorageCleanupJob.Key));

    q.AddTrigger(t => t.WithIdentity($"{nameof(TemporaryStorageCleanupJob)}-cron-trigger").ForJob(TemporaryStorageCleanupJob.Key).StartNow().WithCronSchedule("0 0/45 * * * ?"));

    q.AddTrigger(t => t.WithIdentity($"{nameof(StorageCleanupJob)}-cron-trigger").ForJob(StorageCleanupJob.Key).StartNow().WithCronSchedule("0 0/45 * * * ?"));

    // Startup job
    q.AddTrigger(t => t.WithIdentity($"{nameof(TemporaryStorageCleanupJob)}-startup").ForJob(TemporaryStorageCleanupJob.Key).StartNow());

    q.AddTrigger(t => t.WithIdentity($"{nameof(StorageCleanupJob)}-startup").ForJob(StorageCleanupJob.Key).StartNow());

});

// Transcoder
builder.Services.AddTranscoder();

// Connection string retrievals, repository and cors
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builderOpts =>
            {
                builderOpts.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });
    var connectionString = builder.Configuration.GetConnectionString("OsDbContext");
    if (connectionString == null)
    {
        builder.Services.AddScoped<IRepository, MockRepository>();
    }
    else
    {
        builder.Services.AddDbContext<OsDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("OsDbContext"));
        });
        builder.Services.AddScoped<IRepository, SqlRepository>();
    }
}
else
{
    builder.Services.AddDbContext<OsDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("OsDbContext"));
    });
    builder.Services.AddScoped<IRepository, SqlRepository>();
}

// Scheduler
builder.Services.AddScheduler();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();