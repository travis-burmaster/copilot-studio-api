using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.CopilotStudio;
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

// Configure Copilot Studio Client with DefaultAzureCredential
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    TenantId = directToEngineSettings?.TenantId
});

// Register Copilot Studio service
builder.Services.AddSingleton<ICopilotStudioService>(sp =>
{
    return new CopilotStudioClient(
        directToEngineSettings.AppClientId,
        credential);
});

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