public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string>? Context { get; set; }
}