using MiniCtf.Models;
using System.Text;

namespace MiniCtf.Puzzles;

public class DecodeChallenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private string _plaintext = string.Empty;
    private string _encoded = string.Empty;
    private string[] _pipeline = Array.Empty<string>();

    public string Title => _def.Title;

    public DecodeChallenge(ChallengeDefinition def)
    {
        _def = def;
        InitializeProblem();
    }

    public ChallengeResult Run()
    {
        if (IsRefreshEachRun()) InitializeProblem();

        Console.WriteLine(_def.Prompt);
        Console.WriteLine();

        // Show helpful parameters for solvable transforms
        var mode = _def.Parameters?.GetValueOrDefault("mode");
        var vigKey = _def.Parameters?.GetValueOrDefault("vigKey");
        var rails = _def.Parameters?.GetValueOrDefault("rails");

        if (!string.IsNullOrWhiteSpace(vigKey)) Console.WriteLine($"Key (Vigenere): {vigKey}");
        if (!string.IsNullOrWhiteSpace(rails)) Console.WriteLine($"Rails (Rail Fence): {rails}");

        Console.WriteLine("Encoded: " + _encoded);
        Console.Write("Enter original plaintext: ");
        var input = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.Equals(input, _plaintext, StringComparison.Ordinal))
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            return new ChallengeResult(true, flag, "Decoded successfully");
        }
        return new ChallengeResult(false, null, "Incorrect plaintext");
    }

    private void InitializeProblem()
    {
        _pipeline = GetPipeline();
        _plaintext = $"flag{{dec_{MiniCtf.Utils.RandomText.Token(10)}}}";
        _encoded = ApplyPipeline(_plaintext, _pipeline);
    }

    private string[] GetPipeline()
    {
        var pipe = _def.Parameters?.GetValueOrDefault("pipeline");
        if (!string.IsNullOrWhiteSpace(pipe))
        {
            return pipe.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                       .Select(p => p.ToLowerInvariant()).ToArray();
        }
        var single = _def.Parameters?.GetValueOrDefault("mode")?.ToLowerInvariant();
        return string.IsNullOrWhiteSpace(single) ? new[] { "base64" } : new[] { single };
    }

    private string ApplyPipeline(string input, string[] steps)
    {
        string result = input;
        foreach (var step in steps)
        {
            result = step switch
            {
                "base64" => EncodeBase64(result),
                "hex" => EncodeHex(result),
                "url" => EncodeUrl(result),
                "rot13" => Rot13(result),
                "rot47" => Rot47(result),
                "atbash" => Atbash(result),
                "reverse" => Reverse(result),
                "ascii-dec" => AsciiCodes(result, 10),
                "ascii-oct" => AsciiCodes(result, 8),
                "binary8" => Binary8(result),
                "leetspeak" => LeetSpeak(result),
                "vigenere" => Vigenere(result, _def.Parameters?.GetValueOrDefault("vigKey") ?? "ctf"),
                "railfence" => RailFence(result, int.TryParse(_def.Parameters?.GetValueOrDefault("rails"), out var r) ? Math.Max(2, r) : 3),
                _ => result
            };
        }
        return result;
    }

    private static string EncodeBase64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
    private static string EncodeHex(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
    private static string EncodeUrl(string s) => Uri.EscapeDataString(s);

    private static string Rot13(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (ch is >= 'a' and <= 'z') sb.Append((char)('a' + ((ch - 'a' + 13) % 26)));
            else if (ch is >= 'A' and <= 'Z') sb.Append((char)('A' + ((ch - 'A' + 13) % 26)));
            else sb.Append(ch);
        }
        return sb.ToString();
    }

    private static string Rot47(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (ch >= 33 && ch <= 126)
                sb.Append((char)(33 + ((ch - 33 + 47) % 94)));
            else sb.Append(ch);
        }
        return sb.ToString();
    }

    private static string Atbash(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (ch is >= 'a' and <= 'z') sb.Append((char)('z' - (ch - 'a')));
            else if (ch is >= 'A' and <= 'Z') sb.Append((char)('Z' - (ch - 'A')));
            else sb.Append(ch);
        }
        return sb.ToString();
    }

    private static string Reverse(string s)
    {
        var arr = s.ToCharArray(); Array.Reverse(arr); return new string(arr);
    }

    private static string AsciiCodes(string s, int numberBase)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        return string.Join(" ", bytes.Select(b => Convert.ToString(b, numberBase)));
    }

    private static string Binary8(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        return string.Join(" ", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

    private static string LeetSpeak(string s)
    {
        return s
            .Replace("a", "4").Replace("A", "4")
            .Replace("e", "3").Replace("E", "3")
            .Replace("i", "1").Replace("I", "1")
            .Replace("o", "0").Replace("O", "0")
            .Replace("s", "5").Replace("S", "5")
            .Replace("t", "7").Replace("T", "7");
    }

    private static string Vigenere(string s, string key)
    {
        if (string.IsNullOrEmpty(key)) key = "ctf";
        var sb = new StringBuilder(s.Length);
        int ki = 0;
        foreach (var ch in s)
        {
            var k = key[ki % key.Length];
            int shift = char.IsLetter(k) ? (char.ToLowerInvariant(k) - 'a') : 0;
            if (char.IsLetter(ch))
            {
                bool up = char.IsUpper(ch);
                char a = up ? 'A' : 'a';
                sb.Append((char)(a + ((ch - a + shift) % 26)));
                ki++;
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }

    private static string RailFence(string s, int rails)
    {
        if (rails < 2) rails = 2;
        var fence = new StringBuilder[rails];
        for (int i = 0; i < rails; i++) fence[i] = new StringBuilder();
        int rail = 0; int dir = 1;
        foreach (var ch in s)
        {
            fence[rail].Append(ch);
            rail += dir;
            if (rail == rails - 1 || rail == 0) dir *= -1;
        }
        var result = new StringBuilder(s.Length);
        foreach (var row in fence) result.Append(row.ToString());
        return result.ToString();
    }

    private bool IsRefreshEachRun()
    {
        return _def.Parameters?.GetValueOrDefault("refreshEachRun")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }
}
