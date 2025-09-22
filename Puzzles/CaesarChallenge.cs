using MiniCtf.Models;
using System.Text;

namespace MiniCtf.Puzzles;

public class CaesarChallenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private int _shift;
    private string _plaintext = string.Empty;
    private string _ciphertext = string.Empty;

    public string Title => _def.Title;

    public CaesarChallenge(ChallengeDefinition def)
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
        Console.WriteLine("Ciphertext: " + _ciphertext);
    Console.WriteLine("Hint: It's a classic Caesar shift. Try brute forcing shifts 0-25.");
        Console.Write("Enter original plaintext: ");
        var input = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.Equals(input, _plaintext, StringComparison.Ordinal))
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            return new ChallengeResult(true, flag, "Recovered plaintext");
        }
        return new ChallengeResult(false, null, "Incorrect plaintext");
    }

    private void InitializeProblem()
    {
        var dynamicMode = _def.Parameters?.GetValueOrDefault("dynamic")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
        if (dynamicMode)
        {
            _plaintext = $"flag{{caesar_{MiniCtf.Utils.RandomText.Token(8)}}}";
            // Random shift 1-25
            _shift = Random.Shared.Next(1, 26);
            _ciphertext = Shift(_plaintext, _shift);
            return;
        }

        _plaintext = _def.Parameters?.GetValueOrDefault("plaintext") ?? "flag{caesar_r0cks}";
        if (int.TryParse(_def.Parameters?.GetValueOrDefault("shift"), out var s))
        {
            _shift = s;
            _ciphertext = Shift(_plaintext, _shift);
        }
        else
        {
            _ciphertext = _def.Parameters?.GetValueOrDefault("ciphertext") ?? Shift(_plaintext, 13);
            _shift = 0;
        }
    }

    private bool IsRefreshEachRun()
    {
        return _def.Parameters?.GetValueOrDefault("refreshEachRun")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string Shift(string text, int shift)
    {
        var sb = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (char.IsLetter(ch))
            {
                bool isUpper = char.IsUpper(ch);
                char a = isUpper ? 'A' : 'a';
                int offset = (ch - a + shift) % 26;
                if (offset < 0) offset += 26;
                sb.Append((char)(a + offset));
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
}
