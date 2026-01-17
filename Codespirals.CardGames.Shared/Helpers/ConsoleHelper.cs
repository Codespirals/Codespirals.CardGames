namespace Codespirals.CardGames;
public static class ConsoleHelper
{
    public static int ReadUntilInt(int min = 0, int max = int.MaxValue)
    {
        int number;
        while (!int.TryParse(Console.ReadLine(), out number) && number < min && number > max)
            Console.WriteLine("Not a valid input. Try again.");
        return number;
    }
    public static string ReadUntilAccepted()
    {
        var result = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine("Not a valid input. Try again.");
            result = Console.ReadLine();
        }
        return result!;
    }
    public static string ReadUntilAccepted(params string[] acceptableValues)
    {
        var result = Console.ReadLine();
        result = result?.Replace(" ", "");
        while (string.IsNullOrWhiteSpace(result) || !acceptableValues.Any(v => v.Equals(result, StringComparison.CurrentCultureIgnoreCase)))
        {
            Console.WriteLine("Not a valid input. Try again.");
            result = Console.ReadLine();
            result = result?.Replace(" ", "");
        }
        return result!;
    }
    public static string ReadUntilStartsWith(params char[] acceptableValues)
    {
        var result = Console.ReadLine();
        result = result?.Replace(" ", "");
        while (string.IsNullOrWhiteSpace(result) || !acceptableValues.Any(c => result.StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Not a valid input. Try again.");
            result = Console.ReadLine();
            result = result?.Replace(" ", "");
        }
        return result!;
    }
    public static void SeperatorLine(char lineChar = '-')
        => Console.WriteLine(new string(lineChar, 40));

    public static void SetColorForPlayer(int index)
    {
        Console.ForegroundColor = index switch
        {
            0 => ConsoleColor.Red,
            1 => ConsoleColor.Green,
            2 => ConsoleColor.Blue,
            3 => ConsoleColor.Yellow,
            4 => ConsoleColor.Magenta,
            5 => ConsoleColor.Gray,
            6 => ConsoleColor.Cyan,
            7 => ConsoleColor.DarkYellow,
            _ => ConsoleColor.White,
        };
    }

    public static int GetPlayerCount(int min, int max)
    {
        Console.WriteLine("How many players are playing?");
        Console.WriteLine($"Enter a number between {min} and {max}:");
        return ConsoleHelper.ReadUntilInt(min, max);
    }

    public static void AskToNamePlayers<TPlayer>(IEnumerable<TPlayer> players)
        where TPlayer : IPlayer

    {
        Console.WriteLine($"Do you want to name the players?");
        var answer = ConsoleHelper.ReadUntilStartsWith('y', 'n');
        if (answer.StartsWith('n'))
            return;
        var playerList = players.ToList();
        foreach (var player in players)
        {
            ConsoleHelper.SetColorForPlayer(playerList.IndexOf(player));
            var oldName = player.Name;
            Console.WriteLine($"Set {oldName}'s new name!");
            player.Name = ConsoleHelper.ReadUntilAccepted();
            Console.WriteLine($"{oldName} is now {player.Name}!");
        }
    }
}