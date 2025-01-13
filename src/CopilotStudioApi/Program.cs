using Microsoft.PowerPlatform.Dataverse.Client;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

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

// Get logger for startup
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());
var logger = loggerFactory.CreateLogger("Startup");

// Validate settings
if (directToEngineSettings == null)
{
    throw new InvalidOperationException("DirectToEngineSettings section is missing from configuration.");
}

logger.LogInformation("Configuration loaded:");
logger.LogInformation($"EnvironmentId: {directToEngineSettings.EnvironmentId}");
logger.LogInformation($"TenantId: {directToEngineSettings.TenantId}");
logger.LogInformation($"AppClientId: {directToEngineSettings.AppClientId}");

if (string.IsNullOrEmpty(directToEngineSettings.EnvironmentId))
{
    throw new InvalidOperationException("EnvironmentId is required in DirectToEngineSettings.");
}

if (string.IsNullOrEmpty(directToEngineSettings.AppClientId))
{
    throw new InvalidOperationException("AppClientId is required in DirectToEngineSettings.");
}

if (string.IsNullOrEmpty(directToEngineSettings.TenantId))
{
    throw new InvalidOperationException("TenantId is required in DirectToEngineSettings.");
}

// Configure Dataverse ServiceClient
try
{
    var tenantId = directToEngineSettings.TenantId.TrimEnd('/');
    var orgUrl = directToEngineSettings.EnvironmentId.Contains(".")
        ? $"https://{directToEngineSettings.EnvironmentId}"
        : $"https://{directToEngineSettings.EnvironmentId}.crm.dynamics.com";

    logger.LogInformation($"Connecting to Dataverse environment: {orgUrl}");
    logger.LogInformation($"Using AppId: {directToEngineSettings.AppClientId}");
    logger.LogInformation($"Using TenantId: {tenantId}");

    // Create connection string
    var connectionString = string.Format(
        "AuthType=ClientSecret;" +
        "Url={0};" +
        "ClientId={1};" +
        "ClientSecret={2};" +
        "RequireNewInstance=true;" +
        "TokenCacheStorePath=.;",
        orgUrl,
        directToEngineSettings.AppClientId,
        directToEngineSettings.ClientSecret
    );

    logger.LogInformation($"Using connection string template: {connectionString.Replace(directToEngineSettings.ClientSecret, "[REDACTED]")}")

    // Create and test the connection
    var clientConfig = new ServiceClient(connectionString, logger);

    if (!clientConfig.IsReady)
    {
        var error = clientConfig.LastError;
        logger.LogError($"Failed to initialize ServiceClient: {error}");
        throw new InvalidOperationException($"Failed to initialize ServiceClient: {error}");
    }

    builder.Services.AddSingleton(_ => clientConfig);
    logger.LogInformation("Successfully connected to Dataverse");
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to initialize Dataverse connection");
    throw;
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