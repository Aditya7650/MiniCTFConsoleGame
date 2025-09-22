namespace MiniCtf.Models;

public interface IChallenge
{
    string Title { get; }
    ChallengeResult Run();
}

public record ChallengeResult(bool Success, string? Flag = null, string? Message = null);

public class ChallengeDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // base64 | caesar | weakregex
    public string Prompt { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public Dictionary<string, string>? Parameters { get; set; }
}
