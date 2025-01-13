using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.CopilotStudio.Client;
using Azure.Identity;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class CopilotController : ControllerBase
{
    private readonly ICopilotStudioClient _copilotClient;
    private readonly DirectToEngineSettings _settings;
    private readonly ILogger<CopilotController> _logger;

    public CopilotController(
        ICopilotStudioClient copilotClient,
        IOptions<DirectToEngineSettings> settings,
        ILogger<CopilotController> logger)
    {
        _copilotClient = copilotClient;
        _settings = settings.Value;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> GetCopilotResponse([FromBody] ChatRequest request)
    {
        try
        {
            var copilotRequest = new ConversationRequest
            {
                Message = request.Message,
                Context = request.Context,
                BotIdentifier = _settings.BotIdentifier
            };

            var response = await _copilotClient.ConversationAsync(
                _settings.EnvironmentId,
                copilotRequest);

            return Ok(new ChatResponse
            {
                Message = response.Message,
                ConversationId = response.ConversationId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Copilot response");
            return StatusCode(500, "Error processing request");
        }
    }
}