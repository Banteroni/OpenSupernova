using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Services.Repository;
using Quartz;
using OS.API.Extensions;
using OS.Services.Jobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using OS.Services.Auth;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
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
      options.AddPolicy("Contributor",
      policy => policy.RequireClaim(ClaimTypes.Role, ["0", "1"])));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

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