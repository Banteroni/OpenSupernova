using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Services.Repository;
using Quartz;
using OS.API.Extensions;
using OS.Services.Jobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OS.Services.Mappers;
using OS.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(x =>
{
    x.ClearProviders();
    x.AddConsole();
});

builder.Services.AddScoped<IDtoMapper, DtoMapper>();

// Storage
builder.Services.AddStorage();

// Quartz
builder.Services.AddQuartz(q =>
{
    q.AddJob<ImportTracksJob>(j => j.StoreDurably().WithIdentity(ImportTracksJob.Key));

    q.AddJob<TemporaryStorageCleanupJob>(j => j.WithIdentity(TemporaryStorageCleanupJob.Key));

    q.AddJob<DatabaseCleanup>(j => j.WithIdentity(DatabaseCleanup.Key));

    q.AddTrigger(t => t.WithIdentity($"{nameof(TemporaryStorageCleanupJob)}-cron-trigger").ForJob(TemporaryStorageCleanupJob.Key).StartNow().WithCronSchedule("0 0/45 * * * ?"));

    q.AddTrigger(t => t.WithIdentity($"{nameof(DatabaseCleanup)}-cron-trigger").ForJob(DatabaseCleanup.Key).StartNow().WithCronSchedule("0 0/45 * * * ?"));

    // Startup job
    q.AddTrigger(t => t.WithIdentity($"{nameof(TemporaryStorageCleanupJob)}-startup").ForJob(TemporaryStorageCleanupJob.Key).StartNow());

    q.AddTrigger(t => t.WithIdentity($"{nameof(DatabaseCleanup)}-startup").ForJob(DatabaseCleanup.Key).StartNow());

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
}

var connectionString = builder.Configuration.GetConnectionString("OsDbContext");
builder.Services.AddDbContext<OsDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IRepository, SqlRepository>();

// Scheduler
builder.Services.AddScheduler();

// AES
var aes = Aes.Create();
aes.GenerateKey();
builder.Services.AddSingleton(aes);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(aes.Key)
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Contributor",
      policy => policy.RequireClaim(ClaimTypes.Role, ["Contributor", "Admin"]));
    options.AddPolicy("Admin",
        policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddScoped<PagingMiddleware>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(x =>
{
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
    x.WithExposedHeaders(["Content-Range"]);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<PagingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();