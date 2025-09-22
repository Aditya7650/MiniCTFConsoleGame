using MiniCtf.Models;
using MiniCtf.Puzzles;

namespace MiniCtf.Engine;

public static class ChallengeFactory
{
    public static IChallenge Create(ChallengeDefinition def)
    {
        return def.Type.ToLowerInvariant() switch
        {
            "base64" => new Base64Challenge(def),
            "caesar" => new CaesarChallenge(def),
            "weakregex" => new WeakRegexLoginChallenge(def),
            "hashcrack" => new HashCrackChallenge(def),
            "sqlinjection" => new SqlInjectionChallenge(def),
            "decode" => new DecodeChallenge(def),
            "web" => new WebBypassChallenge(def),
            _ => throw new NotSupportedException($"Unsupported challenge type: {def.Type}")
        };
    }
}
