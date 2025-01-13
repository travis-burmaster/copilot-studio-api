using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.CopilotStudio;
using Microsoft.Extensions.Options;
using Azure.Identity;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class CopilotController : ControllerBase
{
    private readonly ICopilotStudioService _copilotClient;
    private readonly DirectToEngineSettings _settings;
    private readonly ILogger<CopilotController> _logger;

    public CopilotController(
        ICopilotStudioService copilotClient,
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
            var response = await _copilotClient.GetChatMessageAsync(
                request.Message,
                _settings.BotIdentifier,
                _settings.EnvironmentId,
                request.Context);

            return Ok(new ChatResponse
            {
                Message = response,
                ConversationId = Guid.NewGuid().ToString() // Since the new API doesn't return a conversationId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Copilot response");
            return StatusCode(500, "Error processing request");
        }
    }
}