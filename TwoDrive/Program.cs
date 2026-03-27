using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using Serilog;
using TwoDrive.Persistence;
using TwoDrive.Storage;
using Microsoft.Extensions.Azure;
using TwoDrive.Services;
using TwoDrive.Api.Common;
using TwoDrive.Http;
using TwoDrive.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);



// Infrastructure
builder.Services.AddTwoDriveHttp();
builder.Services.AddTwoDriveStorageServices(builder.Configuration);
builder.Services.AddTwoDrivePersistence(builder.Configuration);

// Application
builder.Services.AddTwoDriveServices();

// API
builder.Services.AddHttpContextAccessor();
builder.Services.AddValidation();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddOpenApi();

// CORS setup for development environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(opt =>
    {
        opt.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}

// Logging
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

// Authentication setup with Azure AD
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapEndpoints();

app.Run();
