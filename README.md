# Mini CTF (Console Game)

A lightweight C# console application delivering a text-based Capture The Flag experience with educational cybersecurity puzzles.

## Features

- Base64 decoding challenge
- Caesar cipher cracking challenge
- Weak regex login bypass simulation
- Hash cracking (SHA-256) challenge
- SQL injection bypass simulation
- JSON-driven challenge definitions for easy extension

## Requirements

- .NET 6 SDK or later

## Run

1. Restore and build.
2. Run the console app.

On Windows PowerShell:

```powershell
# From the project folder
dotnet build ; dotnet run
```

## Structure

- `Program.cs` – entry point
- `Engine/` – game loop and challenge factory
- `Puzzles/` – individual challenge implementations
- `Models/` – interfaces and models
- `Data/challenges.json` – challenge definitions

## Adding New Challenges

1. Create a new class in `Puzzles/` implementing `IChallenge` and accepting a `ChallengeDefinition` in the constructor.
2. Register the type in `Engine/ChallengeFactory.cs`.
3. Add a new entry in `Data/challenges.json` with `type`, `title`, `prompt`, `order`, and `parameters`.
   - Note: Flags are generated dynamically via HMAC using the challenge `id`. Do not store static flags in JSON.
   - Optional: Set an environment variable `MINI_CTF_SECRET` to a stable secret if you want flags to be consistent across runs.

## Educational Notes

- Base64: Encoding vs encryption
- Caesar: Brute-force and classical ciphers
- Regex: Pitfalls of using weak or partial-matching regex for auth

## Roadmap

- Scoring, hints, and leaderboard
- Additional puzzles (hash cracking, SQL injection simulation, XSS)
- Packaged puzzle packs for classrooms and training

---
Enjoy and learn!
