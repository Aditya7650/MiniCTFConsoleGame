# Mini CTF (Console Game)

A lightweight C# console application delivering a text-based Capture The Flag experience with educational cybersecurity puzzles.

## Features

- 50 unique CTF challenges (no repetition)
- Multi-mode Decode engine: base64, hex, url, rot13, rot47, atbash, reverse, ascii-dec, ascii-oct, binary8, leetspeak, vigenere, railfence, plus pipelines (e.g., `base64|rot13`)
- WebBypass engine: open-redirect, path-traversal, command injection, XSS, IDOR
- Dynamic per-run challenge content (problems change each execution)
- Dynamic HMAC-based flags (no flags stored in JSON; optional `MINI_CTF_SECRET` for stable flags)
- JSON-driven challenge definitions for easy extension

## Requirements

- .NET 8 SDK

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

## Diagrams

### Architecture (high-level)

```mermaid
graph TD
   A[Program.cs] --> B[CtfGame]
   B --> C[ChallengeFactory]
   C --> D1[Base64Challenge]
   C --> D2[CaesarChallenge]
   C --> D3[WeakRegexLoginChallenge]
   C --> D4[HashCrackChallenge]
   C --> D5[SqlInjectionChallenge]
   C --> D6[DecodeChallenge]
   C --> D7[WebBypassChallenge]
   B --> E[FlagService]
   D1 -->|uses| E
   D2 -->|uses| E
   D3 -->|uses| E
   D4 -->|uses| E
   D5 -->|uses| E
   D6 -->|uses| E
   D7 -->|uses| E
   A --> F[(Data/challenges.json)]
   B --> F
   G[RandomText]:::util
   D1 --> G
   D2 --> G
   D3 --> G
   D4 --> G
   D6 --> G
   classDef util fill:#eef,stroke:#88a,stroke-width:1px
```

### Decode pipelines (how to solve)

```mermaid
flowchart TD
   subgraph Pipeline_base64_rot13
      P[plaintext] --> S1[base64] --> S2[rot13] --> E[shown to player]
   end

   subgraph Solve_reverse_order
      E --> R1[rot13 inverse] --> R2[base64 inverse] --> P
   end
```

### Challenge run (sequence)

```mermaid
sequenceDiagram
   participant U as User
   participant P as Program.cs
   participant G as CtfGame
   participant F as ChallengeFactory
   participant C as IChallenge
   participant FS as FlagService
   U->>P: start
   P->>G: Run()
   G->>F: Create(definition)
   F-->>G: IChallenge instance
   G->>C: Run()
   C->>FS: GenerateFlag(id)
   FS-->>C: flag{...}
   C-->>G: ChallengeResult(Success, Flag)
   G-->>U: Show flag and message
```

## Adding New Challenges

1. Create a new class in `Puzzles/` implementing `IChallenge` and accepting a `ChallengeDefinition` in the constructor.
2. Register the type in `Engine/ChallengeFactory.cs`.
3. Add a new entry in `Data/challenges.json` with `type`, `title`, `prompt`, `order`, and `parameters`.
   - Note: Flags are generated dynamically via HMAC using the challenge `id`. Do not store static flags in JSON.
   - Optional: Set an environment variable `MINI_CTF_SECRET` to a stable secret if you want flags to be consistent across runs.

## Topics

Add these GitHub topics to help others discover the project:

`ctf`, `capture-the-flag`, `security`, `cybersecurity`, `cryptography`, `c`, `dotnet`, `dotnet-8`, `csharp`, `c`, `education`, `learning`, `reverse-engineering`, `web-security`, `encoding`

## Educational Notes

- Base64: Encoding vs encryption
- Caesar: Brute-force and classical ciphers
- Regex: Pitfalls of using weak or partial-matching regex for auth

## Roadmap

- Scoring, hints, and leaderboard
- CI (build/test) and packaged puzzle packs for classrooms and training
- More decode pipelines and web vuln variants

---
Enjoy and learn!
