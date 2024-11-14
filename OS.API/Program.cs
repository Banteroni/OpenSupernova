using Microsoft.EntityFrameworkCore;
using OS.API;
using OS.Data.Context;
using OS.Data.Options;
using OS.Services.Codec;
using OS.Services.Repository;
using Quartz;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(x =>
{
    x.ClearProviders();
    x.AddConsole();
});

StorageSettings storageSettings = new();
TranscodeSettings transcodeSettings = new();
TemporaryStorageSettings temporaryStorageSettings = new();
builder.Configuration.GetSection("TranscodeSettings").Bind(transcodeSettings);
builder.Configuration.GetSection("StorageSettings").Bind(storageSettings);
builder.Configuration.GetSection("TemporaryStorageSettings").Bind(temporaryStorageSettings);
builder.Services.AddStorage(storageSettings, temporaryStorageSettings);
builder.Services.AddSingleton<ITranscoder, FfmpegTranscoder>(x =>
    ActivatorUtilities.CreateInstance<FfmpegTranscoder>(x, transcodeSettings));

builder.Services.AddQuartz();
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
