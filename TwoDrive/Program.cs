using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using TwoDrive.Persistence;
using TwoDrive.Storage;
using Microsoft.Extensions.Azure;
using TwoDrive.Services;
using TwoDrive.Api.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blobServiceUri"]!);
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnectionString:queueServiceUri"]!);
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnectionString:tableServiceUri"]!);
});

// Infrastructure
builder.Services.AddTwoDriveStorageServices();
builder.Services.AddTwoDrivePersistence(builder.Configuration);

// Application
builder.Services.AddTwoDriveServices();

// API
builder.Services.AddValidation();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapEndpoints();

app.Run();

public partial class Program;
