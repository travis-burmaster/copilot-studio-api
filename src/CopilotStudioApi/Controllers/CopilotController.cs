using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class CopilotController : ControllerBase
{
    private readonly ServiceClient _serviceClient;
    private readonly DirectToEngineSettings _settings;
    private readonly ILogger<CopilotController> _logger;

    public CopilotController(
        ServiceClient serviceClient,
        IOptions<DirectToEngineSettings> settings,
        ILogger<CopilotController> logger)
    {
        _serviceClient = serviceClient;
        _settings = settings.Value;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> GetCopilotResponse([FromBody] ChatRequest request)
    {
        try
        {
            var connectionString = $"AuthType=OAuth;Url=https://{_settings.EnvironmentId}.crm.dynamics.com;AppId={_settings.AppClientId};LoginPrompt=Auto";
            
            // Create the request entity
            var chatRequest = new Microsoft.Xrm.Sdk.Entity("powervirtualagent_session")
            {
                ["powervirtualagent_botid"] = _settings.BotIdentifier,
                ["powervirtualagent_message"] = request.Message
            };

            // If there's context, add it
            if (request.Context != null && request.Context.Any())
            {
                chatRequest["powervirtualagent_context"] = JsonSerializer.Serialize(request.Context);
            }

            // Send the request
            var response = await _serviceClient.CreateAsync(chatRequest);

            // Get the response message
            var responseMessage = await _serviceClient.RetrieveAsync(response.EntityReference, 
                new Microsoft.Xrm.Sdk.Query.ColumnSet("powervirtualagent_responsemessage"));

            return Ok(new ChatResponse
            {
                Message = responseMessage.GetAttributeValue<string>("powervirtualagent_responsemessage"),
                ConversationId = response.Id.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Copilot response");
            return StatusCode(500, "Error processing request");
        }
    }
}