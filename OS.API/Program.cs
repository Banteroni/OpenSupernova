using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Data.Options;
using OS.Services.Codec;
using OS.Services.Repository;
using Quartz;
using OS.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(x =>
{
    x.ClearProviders();
    x.AddConsole();
});
builder.Services.AddStorage();
builder.Services.AddQuartz();

TranscodeSettings transcodeSettings = new();
builder.Configuration.GetSection("TranscodeSettings").Bind(transcodeSettings);
builder.Services.AddSingleton<ITranscoder, FfmpegTranscoder>(x =>
    ActivatorUtilities.CreateInstance<FfmpegTranscoder>(x, transcodeSettings));

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
