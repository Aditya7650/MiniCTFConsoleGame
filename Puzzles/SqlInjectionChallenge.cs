using MiniCtf.Models;
using System.Text.RegularExpressions;

namespace MiniCtf.Puzzles;

public class SqlInjectionChallenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private string _userParam = "username";
    private string _passParam = "password";
    private string _table = "users";

    public string Title => _def.Title;

    public SqlInjectionChallenge(ChallengeDefinition def)
    {
        _def = def;
        _userParam = def.Parameters?.GetValueOrDefault("userParam") ?? "username";
        _passParam = def.Parameters?.GetValueOrDefault("passParam") ?? "password";
        _table = def.Parameters?.GetValueOrDefault("table") ?? "users";
    }

    public ChallengeResult Run()
    {
        Console.WriteLine(_def.Prompt);
        Console.WriteLine();
        Console.WriteLine("Simulated vulnerable query:");
        Console.WriteLine($"SELECT * FROM {_table} WHERE {_userParam} = '" + "{user}" + "' AND {_passParam} = '" + "{pass}" + "';");
        Console.WriteLine("Goal: Bypass login without knowing the real password using an injection payload.");
        Console.WriteLine("Tip: Try closing the string and adding an OR condition.");

        Console.Write("Enter username: ");
        var user = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter password: ");
        var pass = Console.ReadLine() ?? string.Empty;

        // Naive evaluator: string-concat SQL leading to injection
        var query = $"SELECT * FROM {_table} WHERE {_userParam} = '{user}' AND {_passParam} = '{pass}';";

        // Consider login bypassed if payload would change semantics to always true around password
        // Classic patterns: ' OR '1'='1, ' OR 1=1--, ' OR 1=1 #, etc.
        bool bypass = Regex.IsMatch(pass, @"'\s*OR\s*'?1'?\s*=\s*'?1'?(?:\s*--|\s*#|\s*$)", RegexOptions.IgnoreCase)
                   || Regex.IsMatch(user, @"'\s*OR\s*'?1'?\s*=\s*'?1'?(?:\s*--|\s*#|\s*$)", RegexOptions.IgnoreCase);

        if (bypass)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            Console.WriteLine("Login bypassed via SQL injection!");
            // Optionally display the query to show the effect (educational)
            Console.WriteLine("Constructed query: " + query);
            return new ChallengeResult(true, flag, "Bypassed via SQLi");
        }

        Console.WriteLine("Login failed. The query remained restrictive: " + query);
        return new ChallengeResult(false, null, "No injection detected");
    }
}
