using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Data.Options;
using OS.Services.Codec;
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
builder.Services.AddQuartz(x =>
{
    x.UsePersistentStore(store =>
    {
        store.UseProperties = true;
        var quartzConnectionString = builder.Configuration.GetConnectionString("QuartzDbContext");
        if (quartzConnectionString != null)
        {
            store.UseSqlServer(quartzConnectionString);
        }
    });
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

// Cors
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

// Transcoder
builder.Services.AddTranscoder();

if (builder.Environment.IsDevelopment())
{
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

// Get all the BaseJob implementations
var baseJobTypes = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => typeof(BaseJob).IsAssignableFrom(p) && !p.IsAbstract);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();