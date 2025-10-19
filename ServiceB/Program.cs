var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // force app to listen on port 80
});

var app = builder.Build();

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Welcome to Service B!");
app.MapGet("/health", () => Results.Ok("Service B is a live!"));

app.Run();
