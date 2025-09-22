using MiniCtf.Models;
using System.Text;

namespace MiniCtf.Puzzles;

public class Base64Challenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private string _encoded = string.Empty;
    private string _plaintext = string.Empty;

    public string Title => _def.Title;

    public Base64Challenge(ChallengeDefinition def)
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
        Console.WriteLine("Base64: " + _encoded);
        Console.Write("Enter decoded text: ");
        var input = Console.ReadLine()?.Trim() ?? string.Empty;

        var expected = _plaintext;
        if (input == expected)
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            return new ChallengeResult(true, flag, "Correct");
        }
        return new ChallengeResult(false, null, "Incorrect decode");
    }

    private void InitializeProblem()
    {
        var dynamicMode = _def.Parameters?.GetValueOrDefault("dynamic")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
        if (dynamicMode)
        {
            // Generate a random token each time
            var token = MiniCtf.Utils.RandomText.Token(10);
            _plaintext = $"flag{{b64_{token}}}";
            _encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(_plaintext));
            return;
        }

        var encodedFromJson = _def.Parameters?.GetValueOrDefault("encoded");
        if (!string.IsNullOrWhiteSpace(encodedFromJson))
        {
            _encoded = encodedFromJson!;
            try
            {
                _plaintext = Encoding.UTF8.GetString(Convert.FromBase64String(_encoded));
            }
            catch
            {
                _plaintext = string.Empty;
            }
        }
        else
        {
            _plaintext = _def.Parameters?.GetValueOrDefault("plaintext") ?? "flag{hello_world}";
            _encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(_plaintext));
        }
    }

    private bool IsRefreshEachRun()
    {
        return _def.Parameters?.GetValueOrDefault("refreshEachRun")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }
}
