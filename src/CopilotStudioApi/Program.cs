using Microsoft.PowerPlatform.Dataverse.Client;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

// Configure DirectToEngineSettings
var directToEngineSettings = builder.Configuration.GetSection("DirectToEngineSettings")
    .Get<DirectToEngineSettings>();
builder.Services.Configure<DirectToEngineSettings>(
    builder.Configuration.GetSection("DirectToEngineSettings"));

// Configure Dataverse ServiceClient
if (directToEngineSettings != null)
{
    var connectionString = $"AuthType=OAuth;" +
                          $"Url=https://{directToEngineSettings.EnvironmentId}.crm.dynamics.com;" +
                          $"AppId={directToEngineSettings.AppClientId};" +
                          $"Authority={directToEngineSettings.Authority};" +
                          $"TenantId={directToEngineSettings.TenantId};" +
                          $"LoginPrompt=Auto";

    builder.Services.AddSingleton(_ => new ServiceClient(connectionString));
}
else
{
    throw new InvalidOperationException("DirectToEngineSettings is not configured properly.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();