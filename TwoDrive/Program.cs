using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using TwoDrive.Persistence;
using TwoDrive.Storage;
using Microsoft.Extensions.Azure;
using TwoDrive.Services;
using TwoDrive.Api.Common;
using TwoDrive.Http;

var builder = WebApplication.CreateBuilder(args);

// Authentication setup with Azure AD
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Azure Storage clients setup
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blobServiceUri"]!);
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnectionString:queueServiceUri"]!);
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnectionString:tableServiceUri"]!);
});


// Infrastructure
builder.Services.AddTwoDriveStorageServices();
builder.Services.AddTwoDriveHttp();
builder.Services.AddTwoDrivePersistence(builder.Configuration);

// Application
builder.Services.AddTwoDriveServices();

// API
builder.Services.AddHttpContextAccessor();
builder.Services.AddValidation();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddOpenApi();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapEndpoints();

app.Run();


