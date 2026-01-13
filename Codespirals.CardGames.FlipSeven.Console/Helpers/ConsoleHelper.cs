namespace Codespirals.CardGames.FlipSeven;
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
    public static void SeperatorLine(char lineChar = '-')
        => Console.WriteLine(new string(lineChar, 40));

    public static void SetColorForPlayer(int id)
    {
        switch (id)
        {
            case 0:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case 1:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case 2:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case 3:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case 4:
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
            case 5:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case 6:
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case 7:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }
    }
}