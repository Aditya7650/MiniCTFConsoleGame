using MiniCtf.Models;
using System.Text.RegularExpressions;

namespace MiniCtf.Puzzles;

public class WeakRegexLoginChallenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _pattern = string.Empty; // intentionally weak

    public string Title => _def.Title;

    public WeakRegexLoginChallenge(ChallengeDefinition def)
    {
        _def = def;
        InitializeProblem();
    }

    public ChallengeResult Run()
    {
        if (IsRefreshEachRun())
        {
            InitializeProblem();
        }
        Console.WriteLine(_def.Prompt);
        Console.WriteLine();
        Console.WriteLine("Simulated login");
        Console.WriteLine("Validation uses weak regex on username: " + _pattern);
        Console.Write("Enter username: ");
        var user = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter password: ");
        var pass = Console.ReadLine() ?? string.Empty;

        // Insecure check: uses regex on username and ignores password if regex matches
        bool userMatches = Regex.IsMatch(user, _pattern);
        if (userMatches)
        {
            Console.WriteLine("Access granted due to weak regex match! Password not strictly verified.");
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            return new ChallengeResult(true, flag, "Bypassed auth");
        }

        // Fallback: correct strict credentials
        if (user == _username && pass == _password)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            return new ChallengeResult(true, flag, "Legit login");
        }

        return new ChallengeResult(false, null, "Login failed");
    }

    private void InitializeProblem()
    {
        var dynamicMode = _def.Parameters?.GetValueOrDefault("dynamic")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
        _pattern = _def.Parameters?.GetValueOrDefault("pattern") ?? @"^admin.*$";
        if (dynamicMode)
        {
            // Randomize legit creds to avoid static hints; regex remains the exploit.
            _username = "admin";
            _password = MiniCtf.Utils.RandomText.Token(12);
        }
        else
        {
            _username = _def.Parameters?.GetValueOrDefault("username") ?? "admin";
            _password = _def.Parameters?.GetValueOrDefault("password") ?? "password123";
        }
    }

    private bool IsRefreshEachRun()
    {
        return _def.Parameters?.GetValueOrDefault("refreshEachRun")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }
}
