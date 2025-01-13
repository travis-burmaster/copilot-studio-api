using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
            // Create the request entity
            var chatRequest = new Entity("powervirtualagent_session")
            {
                Attributes = new AttributeCollection
                {
                    { "powervirtualagent_botid", _settings.BotIdentifier },
                    { "powervirtualagent_message", request.Message }
                }
            };

            // If there's context, add it
            if (request.Context != null && request.Context.Any())
            {
                chatRequest.Attributes.Add("powervirtualagent_context", JsonSerializer.Serialize(request.Context));
            }

            // Send the request
            var createdEntity = await _serviceClient.CreateAsync(chatRequest);

            // Get the response message using the created record's ID
            var responseEntity = await _serviceClient.RetrieveAsync(
                "powervirtualagent_session",
                createdEntity,
                new ColumnSet("powervirtualagent_responsemessage"));

            return Ok(new ChatResponse
            {
                Message = responseEntity.GetAttributeValue<string>("powervirtualagent_responsemessage"),
                ConversationId = createdEntity.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Copilot response");
            return StatusCode(500, "Error processing request");
        }
    }
}