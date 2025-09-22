using MiniCtf.Models;
using System.Text.Json;

namespace MiniCtf.Engine;

public class CtfGame
{
    private readonly string _dataPath;
    private readonly List<IChallenge> _challenges = new();

    public CtfGame(string dataPath)
    {
        _dataPath = dataPath;
        LoadChallenges();
    }

    private void LoadChallenges()
    {
        if (!File.Exists(_dataPath))
            throw new FileNotFoundException("Challenges data file not found", _dataPath);

        var json = File.ReadAllText(_dataPath);
        var defs = JsonSerializer.Deserialize<List<ChallengeDefinition>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();

        foreach (var def in defs.OrderBy(d => d.Order))
        {
            var challenge = ChallengeFactory.Create(def);
            _challenges.Add(challenge);
        }
    }

    public void Run()
    {
        var foundFlags = new List<string>();
        while (true)
        {
            Console.WriteLine("Select a challenge:");
            for (int i = 0; i < _challenges.Count; i++)
            {
                var c = _challenges[i];
                Console.WriteLine($"  {i + 1}. {c.Title}");
            }
            Console.WriteLine("  0. Exit");
            Console.Write("Enter choice: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input.\n");
                continue;
            }
            if (choice == 0) break;
            if (choice < 1 || choice > _challenges.Count)
            {
                Console.WriteLine("Choice out of range.\n");
                continue;
            }

            var selected = _challenges[choice - 1];
            Console.Clear();
            Console.WriteLine($"=== {selected.Title} ===\n");
            var result = selected.Run();
            Console.WriteLine();
            if (result.Success && result.Flag is not null)
            {
                foundFlags.Add(result.Flag);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Correct! Flag captured: " + result.Flag);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Not quite. You can retry from the menu.");
                Console.ResetColor();
            }
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
            Console.Clear();
        }

        Console.WriteLine($"You captured {foundFlags.Count}/{_challenges.Count} flags.");
        if (foundFlags.Count > 0)
        {
            Console.WriteLine("Flags:");
            foreach (var f in foundFlags)
                Console.WriteLine(" - " + f);
        }
    }
}
