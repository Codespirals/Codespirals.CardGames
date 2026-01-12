// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");



void PlayFlipSeven()
{
    Console.WriteLine("Starting a game of Flip 7");
    var game = Codespirals.CardGames.FlipSeven.Game.SetUp(4);
    while(!game.GameOver)
    {
        Console.WriteLine($"Starting round {game.CurrentRound}");
        game.StartRound();
        var currentPlayer = game.GetCurrentPlayer();
        while (game.RoundActive)
        {
            Console.WriteLine($"It's {currentPlayer.Name}'s turn!");
            Console.WriteLine($"What will you do? Flip or bank?");
            var inputSuccess = false;
            while (!inputSuccess)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;
                if (input.Equals("flip", StringComparison.InvariantCultureIgnoreCase))
                {
                    var drawnCard = currentPlayer.Draw();
                    Console.WriteLine($"The player drew {drawnCard.Name}!");
                    if (drawnCard.CardType is Codespirals.CardGames.FlipSeven.CardType.Freeze or Codespirals.CardGames.FlipSeven.CardType.FlipThree)
                    {
                        Console.WriteLine($"Designate a player you want to use this {drawnCard.Name} on!");
                        var playerId = Console.ReadLine();
                    }
                    inputSuccess = true;
                }
                else if (input.Equals("bank", StringComparison.InvariantCultureIgnoreCase))
                {
                    currentPlayer.BankPoints();
                    inputSuccess = true;
                }
            }
            currentPlayer = game.MoveToNextPlayer();
        }
    }
}