using System.ComponentModel.DataAnnotations;

public class DirectToEngineSettings
{
    [Required(ErrorMessage = "EnvironmentId is required")]
    public string EnvironmentId { get; set; } = string.Empty;

    [Required(ErrorMessage = "BotIdentifier is required")]
    public string BotIdentifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "TenantId is required")]
    public string TenantId { get; set; } = string.Empty;

    [Required(ErrorMessage = "AppClientId is required")]
    public string AppClientId { get; set; } = string.Empty;

    [Required(ErrorMessage = "ClientSecret is required")]
    public string ClientSecret { get; set; } = string.Empty;
}