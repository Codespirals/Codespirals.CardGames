namespace Codespirals.CardGames.Poker;
internal class BlackjackInConsole
{
    private readonly BlackJack _game; 
    private BlackjackInConsole(int playerCount)
    {
        _game = BlackJack.SetUp(playerCount);
    }
    public static BlackjackInConsole SetUp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Starting a game of Blackjack");
        return new(ConsoleHelper.GetPlayerCount(1, 8));
    }
    
    public void Start()
    {
        ConsoleHelper.NamePlayers(_game.Players);

        while (!_game.GameOver)
        {
            PlayRound();
        }
        End();
    }
    private void PlayRound()
    {
        Console.ForegroundColor = ConsoleColor.White;
        ConsoleHelper.SeperatorLine('#');
        Console.WriteLine($"Starting round {_game.CurrentRound}");
        _game.StartRound();
        while (_game.RoundActive)
        {
            var currentPlayer = _game.GetCurrentPlayer();
            PlayerTurn(currentPlayer);
            _game.MoveToNextPlayer();
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("The round is over!");
        foreach (var player in _game.Players)
        {
            if (player.HandValue < 22 && _game.Dealer.HandValue < player.HandValue)
            {
                Console.WriteLine($"{player.Name} wins {player.CurrentBet * 2}");
                player.AddWinnings(player.CurrentBet * 2);
            }
        }
        Console.WriteLine("Here are the current scores:");
        foreach (var player in _game.Players.OrderBy(p => p.Points))
        {
            Console.WriteLine($"{player.Name} has {player.Points}!");
        }
        Console.WriteLine();
    }
    private void PlayerTurn(Player<BlackJack> player)
    {
        ConsoleHelper.SeperatorLine();
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"It's {player.Name}'s turn!");
        if (player.Hand.Count != 0)
        {
            Console.WriteLine(player.ToString());
        }
        EvaluateHand(player);
        if (player.HandValue > 17)
            Console.WriteLine($"What will you do? {nameof(Hit)} or {nameof(Stand)}?");
        else
            Console.WriteLine($"What will you do? {nameof(Hit)}, {nameof(DoubleDown)} or {nameof(Stand)}?");
        var input = ConsoleHelper.ReadUntilStartsWith('h', 'd', 's');
        if (input.StartsWith('h'))
        {
            Hit(player);
        }
        else if (input.StartsWith('d'))
        {
            DoubleDown(player);
        }
        else if (input.StartsWith('s'))
        {
            Stand(player);
        }
        Console.WriteLine();
    }
    private void Hit(Player<BlackJack> player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        var drawnCard = _game.Hit(player);
        Console.WriteLine($"{player.Name} drew a {drawnCard.Name}!");
        EvaluateHand(player);
    }
    private void DoubleDown(Player<BlackJack> player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"{player.Name} is doubling down!");
        var drawnCard = _game.DoubleDown(player);
        Console.WriteLine($"{player.Name} drew a {drawnCard.Name}!");
        EvaluateHand(player);
    }
    private void Stand(Player<BlackJack> player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"{player.Name} is standing on {player.HandValue}.");
        player.Stand();
    }
    private static void EvaluateHand(Player<BlackJack> player)
    {
        Console.WriteLine($"{player.Name} currently has {player.HandValue}.");
        if (player.HandValue == 21)
        {
            if (player.HandCount == 2)
            {
                Console.WriteLine($"Blackjack!");
            }
            else
            {
                Console.WriteLine($"21! Nice!");
            }
            player.Stand();
            return;
        }
        else if (player.HandValue > 21)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Oh no... busted");
            player.Bust();
            return;
        }
    }
    private void End()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Game Over");
    }
}
