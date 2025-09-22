using MiniCtf.Engine;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = "Mini CTF Console Game";

Console.WriteLine("=== Mini CTF (Capture The Flag) ===\n");

try
{
    var game = new CtfGame("Data/challenges.json");
    game.Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
    Console.ResetColor();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey(true);
}
