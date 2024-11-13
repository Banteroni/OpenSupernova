using OS.API;
using OS.Data.Options;
using OS.Services.Codec;
using OS.Services.Repository;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

StorageSettings storageSettings = new();
TranscodeSettings transcodeSettings = new();
builder.Configuration.GetSection("TranscodeSettings").Bind(transcodeSettings);
builder.Configuration.GetSection("StorageSettings").Bind(storageSettings);
builder.Services.AddStorage(storageSettings);
builder.Services.AddSingleton<ITranscoder, FfmpegTranscoder>(x =>
    ActivatorUtilities.CreateInstance<FfmpegTranscoder>(x, transcodeSettings));

builder.Services.AddQuartz();


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IRepository, MockRepository>();
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
