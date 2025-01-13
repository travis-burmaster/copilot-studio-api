var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DirectToEngineSettings
var directToEngineSettings = builder.Configuration.GetSection("DirectToEngineSettings")
    .Get<DirectToEngineSettings>();
builder.Services.Configure<DirectToEngineSettings>(
    builder.Configuration.GetSection("DirectToEngineSettings"));

// Configure Copilot Studio Client with DefaultAzureCredential
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    TenantId = directToEngineSettings.TenantId
});

builder.Services.AddCopilotStudioClient(options =>
{
    options.Credential = credential;
    options.AppClientId = directToEngineSettings.AppClientId;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();