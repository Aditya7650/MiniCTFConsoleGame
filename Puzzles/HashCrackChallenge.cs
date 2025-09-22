using MiniCtf.Models;
using System.Security.Cryptography;
using System.Text;

namespace MiniCtf.Puzzles;

public class HashCrackChallenge : IChallenge
{
    private readonly ChallengeDefinition _def;
    private string _secret = string.Empty; // dynamic plaintext
    private string _hash = string.Empty;   // SHA-256(secret)

    public string Title => _def.Title;

    public HashCrackChallenge(ChallengeDefinition def)
    {
        _def = def;
        InitializeProblem();
    }

    public ChallengeResult Run()
    {
        if (IsRefreshEachRun()) InitializeProblem();

        Console.WriteLine(_def.Prompt);
        Console.WriteLine();
        Console.WriteLine("Algorithm: SHA-256");
        Console.WriteLine("Hash: " + _hash);
        Console.Write("Enter plaintext that produces this hash: ");
        var guess = Console.ReadLine()?.Trim() ?? string.Empty;

        if (SlowEquals(Hash(guess), _hash))
        {
            var flag = MiniCtf.Engine.FlagService.GenerateFlag(_def.Id);
            return new ChallengeResult(true, flag, "Hash cracked");
        }
        return new ChallengeResult(false, null, "Incorrect plaintext");
    }

    private void InitializeProblem()
    {
        // default dynamic: generate a plausible short token to encourage quick brute forcing
        var length = int.TryParse(_def.Parameters?.GetValueOrDefault("length"), out var l) ? Math.Clamp(l, 4, 10) : 6;
        var prefix = _def.Parameters?.GetValueOrDefault("prefix") ?? "ctf";
        _secret = $"{prefix}_{MiniCtf.Utils.RandomText.Token(length)}";
        _hash = Hash(_secret);
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static bool SlowEquals(string a, string b)
    {
        var ba = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        int diff = ba.Length ^ bb.Length;
        for (int i = 0; i < Math.Min(ba.Length, bb.Length); i++) diff |= ba[i] ^ bb[i];
        return diff == 0;
    }

    private bool IsRefreshEachRun()
    {
        return _def.Parameters?.GetValueOrDefault("refreshEachRun")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }
}
