using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ServiceA;

// Create builder for minimal API and background services
var builder = WebApplication.CreateBuilder(args);

// Register background services
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IBitcoinPriceTracker, BitcoinPriceTracker>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // force app to listen on port 80
});

var app = builder.Build();

// Minimal endpoints
app.MapGet("/", () => "Welcome to Service A!");
app.MapGet("/health", () => Results.Ok("Service A is alive!"));

// Start background services and web server
app.Run();