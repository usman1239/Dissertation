namespace Dissertation.Models.Challenge;

public class ChatMessage(string role, string? text)
{
    public string Role { get; set; } = role;
    public string? Text { get; set; } = text;
}