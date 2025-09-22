using MiniCtf.Models;
using System.Text.RegularExpressions;

namespace MiniCtf.Puzzles;

public class WebBypassChallenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private string _mode = "xss";

    public string Title => _def.Title;

    public WebBypassChallenge(ChallengeDefinition def)
    {
        _def = def;
        _mode = def.Parameters?.GetValueOrDefault("mode")?.ToLowerInvariant() ?? "xss";
    }

    public ChallengeResult Run()
    {
        Console.WriteLine(_def.Prompt);
        Console.WriteLine();
        switch (_mode)
        {
            case "open-redirect":
                return ModeOpenRedirect();
            case "path-traversal":
                return ModePathTraversal();
            case "cmd-injection":
                return ModeCmdInjection();
            case "xss":
                return ModeXss();
            case "idor":
                return ModeIdor();
            default:
                return new ChallengeResult(false, null, "Unknown mode");
        }
    }

    private ChallengeResult ModeOpenRedirect()
    {
        Console.WriteLine("Provide a 'next' URL param that makes the app redirect off-site.");
        Console.Write("next= ");
        var next = Console.ReadLine() ?? string.Empty;
        // Vulnerable allow list: only checks startswith '/'
        bool offsite = next.StartsWith("http://") || next.StartsWith("https://") || next.StartsWith("//");
        if (offsite)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            Console.WriteLine("Redirecting to: " + next);
            return new ChallengeResult(true, flag, "Open redirect");
        }
        return new ChallengeResult(false, null, "Not an external redirect");
    }

    private ChallengeResult ModePathTraversal()
    {
        Console.WriteLine("Provide a filename to read. Hint: Try traversing directories.");
        Console.Write("file= ");
        var file = Console.ReadLine() ?? string.Empty;
        // Naive check blocks only '../..' exactly
        bool traversal = file.Contains("../") || file.Contains("..\\");
        if (traversal)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            Console.WriteLine("Oops, traversal detected. Sensitive file read simulated.");
            return new ChallengeResult(true, flag, "Path traversal");
        }
        return new ChallengeResult(false, null, "No traversal detected");
    }

    private ChallengeResult ModeCmdInjection()
    {
        Console.WriteLine("App runs: ping <host>. Bypass by injecting a second command.");
        Console.Write("host= ");
        var host = Console.ReadLine() ?? string.Empty;
        bool injected = host.Contains("&&") || host.Contains(";") || host.Contains("| ");
        if (injected)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            Console.WriteLine("Command injection detected.");
            return new ChallengeResult(true, flag, "Command injection");
        }
        return new ChallengeResult(false, null, "Looks safe");
    }

    private ChallengeResult ModeXss()
    {
        Console.WriteLine("Comment box reflects input without proper encoding. Craft an XSS payload.");
        Console.Write("comment= ");
        var comment = Console.ReadLine() ?? string.Empty;
        // Detect simple <script> or onerror/onload event handlers
    bool xss = Regex.IsMatch(comment, "<script\\b", RegexOptions.IgnoreCase)
        || Regex.IsMatch(comment, "onerror\\s*=|onload\\s*=", RegexOptions.IgnoreCase)
        || Regex.IsMatch(comment, "<img\\b", RegexOptions.IgnoreCase);
        if (xss)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            Console.WriteLine("XSS payload reflected!");
            return new ChallengeResult(true, flag, "XSS");
        }
        return new ChallengeResult(false, null, "No XSS detected");
    }

    private ChallengeResult ModeIdor()
    {
        Console.WriteLine("Access a resource by ID. Try changing the ID to someone else's.");
        Console.Write("id= ");
        var id = Console.ReadLine() ?? string.Empty;
        // Simulate user id '1001' as yours, anything else as someone else's
        if (id != "1001")
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            Console.WriteLine("IDOR detected: accessed another user's resource.");
            return new ChallengeResult(true, flag, "IDOR");
        }
        return new ChallengeResult(false, null, "Access limited to your own ID");
    }
}
