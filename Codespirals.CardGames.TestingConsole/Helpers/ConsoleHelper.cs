namespace Codespirals.CardGames.TestingConsole;
internal static class ConsoleHelper
{
    public static int ReadUntilInt(int min = 0, int max = int.MaxValue)
    {
        int number;
        while (!int.TryParse(Console.ReadLine(), out number) && number < min && number > max)
            Console.WriteLine("Not a valid input. Try again.");
        return number;
    }
    public static string ReadUntilAccepted(params string[] acceptableValues)
    {
        var result = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(result) || !acceptableValues.Any(v => v.Equals(result, StringComparison.CurrentCultureIgnoreCase)))
        {
            Console.WriteLine("Not a valid input. Try again.");
            result = Console.ReadLine();
        }
        return result!;
    }
    public static int ReadUntilAcceptedInt(params int[] acceptableValues)
    {
        int number;
        while (!int.TryParse(Console.ReadLine(), out number) && !acceptableValues.Any(v => v == number))
            Console.WriteLine("Not a valid input. Try again.");
        return number;
    }
}