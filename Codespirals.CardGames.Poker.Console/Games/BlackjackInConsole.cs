using Codespirals.CardGames.Poker.BlackJack;

namespace Codespirals.CardGames.Poker;
internal class BlackjackInConsole
{
    private readonly BlackJackGame _game; 
    private BlackjackInConsole(int playerCount)
    {
        _game = BlackJackGame.SetUp(playerCount);
    }
    public static BlackjackInConsole SetUp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        var game = new BlackjackInConsole(1);
        //ConsoleHelper.AskToNamePlayers(game._game.Players);
        return game;
    }
    
    public void Start()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Starting a new game of blackjack.");
        while (!_game.GameOver)
        {
            PlayRound();
        }
        End();
    }
    private void PlayRound()
    {
        _game.StartRound();
        Console.ForegroundColor = ConsoleColor.White;
        ConsoleHelper.SeperatorLine('#');
        Console.WriteLine($"Starting round {_game.CurrentRound}");
        Console.WriteLine($"The current buy in is {_game.BuyIn}");
        Console.WriteLine(_game.Prompt);
        while (_game.RoundActive)
        {
            ShowDealer();
            PlayerTurn();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Turn over.");
        }
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        ConsoleHelper.SeperatorLine('*');
        Console.WriteLine($"Round {_game.CurrentRound} is over! Calculating winnings!");

        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(_game.Dealer));
        Console.WriteLine($"The dealer had {string.Join('|', _game.Dealer.Hand.Select(c => c.Name))} ({_game.Dealer.HandValue})");

        foreach (var item in _game.CalculateCurrentPotentialPointGain())
        {
            ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(item.Player));
            Console.WriteLine($"{item.Player.Name} had {string.Join('|', item.Player.Hand.Select(c => c.Name))} ({item.Player.HandValue})");
            if (item.Winnings < 1)
                Console.WriteLine($"{item.Player.Name} lost...");
            else if (item.Winnings == _game.BuyIn)
                Console.WriteLine($"{item.Player.Name} drew with the dealer! +{item.Winnings}");
            else if (item.Player.HandValue == _game.BlackJackScore && item.Player.Hand.Count == 2)
                Console.WriteLine($"{item.Player.Name} got a blackjack! +{item.Winnings}");
            else
                Console.WriteLine($"{item.Player.Name} won! +{item.Winnings}");
        }
        _game.PayOut();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Here are the current scores:");
        foreach (var player in _game.Players.Except([_game.Dealer]).OrderBy(p => p.TotalPoints))
        {
            ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
            Console.WriteLine($"{player.Name} has ${player.TotalPoints}!");
        }
    }
    private void ShowDealer()
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(_game.Dealer));
        ConsoleHelper.SeperatorLine();
        Console.WriteLine($"The dealer has {_game.Dealer.Hand.First().Name} and {_game.Dealer.Hand.Count -1} hidden card(s).");
    }
    private void PlayerTurn()
    {
        var player = _game.CurrentPlayer;
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        ConsoleHelper.SeperatorLine();
        Console.WriteLine($"It's {player.Name}'s turn!");
        Console.WriteLine($"They have {string.Join("|", player.Hand.Select(c => c.Name))} ({player.HandValue})");
        Console.WriteLine(_game.Prompt);
        if (player.HandValue > 17)
            Console.WriteLine($"What will you do? {nameof(Hit)} or {nameof(Stand)}?");
        else
            Console.WriteLine($"What will you do? {nameof(Hit)}, {nameof(DoubleDown)} or {nameof(Stand)}?");
        var input = ConsoleHelper.ReadUntilStartsWith('h', 'd', 's');
        if (input.StartsWith('h'))
            Hit(player);
        else if (input.StartsWith('d'))
            DoubleDown(player);
        else if (input.StartsWith('s'))
            Stand(player);

        if (player.IsBusted)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Oh no... busted");
            return;
        }
    }
    private void Hit(BlackJackPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        var drawnCard = _game.Hit();
        Console.WriteLine($"{player.Name} drew a {drawnCard?.Name}!");
        Console.WriteLine($"{player.Name} currently has {player.HandValue}.");
    }
    private void DoubleDown(BlackJackPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"{player.Name} is doubling down!");
        var drawnCard = _game.DoubleDown();
        Console.WriteLine($"{player.Name} drew a {drawnCard?.Name}!");
        Console.WriteLine($"{player.Name} currently has {player.HandValue}.");
    }
    private void Stand(BlackJackPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"{player.Name} is standing on {player.HandValue}.");
        _game.Stand();
    }

    private void End()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Game Over");
    }
}
