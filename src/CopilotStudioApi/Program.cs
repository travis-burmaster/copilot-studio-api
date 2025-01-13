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

// Validate settings
if (directToEngineSettings == null ||
    string.IsNullOrEmpty(directToEngineSettings.EnvironmentId) ||
    string.IsNullOrEmpty(directToEngineSettings.AppClientId) ||
    string.IsNullOrEmpty(directToEngineSettings.TenantId))
{
    throw new InvalidOperationException("DirectToEngineSettings is not configured properly.");
}

// Construct the authority URL if not provided
if (string.IsNullOrEmpty(directToEngineSettings.Authority))
{
    directToEngineSettings.Authority = $"https://login.microsoftonline.com/{directToEngineSettings.TenantId}";
}

// Configure Dataverse ServiceClient
try
{
    var connectionString = $"AuthType=OAuth;" +
                          $"Url=https://{directToEngineSettings.EnvironmentId}.crm.dynamics.com;" +
                          $"AppId={directToEngineSettings.AppClientId};" +
                          $"RedirectUri=http://localhost;" +
                          $"LoginPrompt=Always;" +
                          $"TokenCacheStorePath=.;" +
                          $"Authority={directToEngineSettings.Authority}";

    // Test the connection string
    var testClient = new ServiceClient(connectionString);
    if (!testClient.IsReady)
    {
        throw new InvalidOperationException("Failed to initialize ServiceClient: " + testClient.LastError);
    }

    builder.Services.AddSingleton(_ => testClient);
}
catch (Exception ex)
{
    throw new InvalidOperationException($"Failed to initialize Dataverse connection: {ex.Message}", ex);
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