using System.Security.Cryptography;
using System.Text;

namespace MiniCtf.Engine;

public static class FlagService
{
    private static readonly byte[] Secret;

    static FlagService()
    {
        var env = Environment.GetEnvironmentVariable("MINI_CTF_SECRET");
        if (string.IsNullOrWhiteSpace(env))
        {
            // Ephemeral per-process secret; can be overridden via env var for stable flags.
            env = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }
        Secret = Encoding.UTF8.GetBytes(env);
    }

    public static string GenerateFlag(string challengeId)
    {
        using var hmac = new HMACSHA256(Secret);
        var bytes = Encoding.UTF8.GetBytes(challengeId);
        var hash = hmac.ComputeHash(bytes);
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        var shortHex = hex[..12];
        return $"flag{{{challengeId}-{shortHex}}}";
    }
}
