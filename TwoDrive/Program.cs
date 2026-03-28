using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using Serilog;
using TwoDrive.Persistence;
using TwoDrive.Storage;
using TwoDrive.Services;
using TwoDrive.Api.Common;
using TwoDrive.Http;
using TwoDrive.Api.Middlewares;
using TwoDrive.Api.Configs;

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

// API configuration
builder
    .ConfigureCORS()
    .ConfigureAuthentication()
    .ConfigureLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<HTTPLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapEndpoints();

app.Run();
