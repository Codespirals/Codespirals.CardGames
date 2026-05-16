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
        var game = new BlackjackInConsole(ConsoleHelper.GetPlayerCount(1, 8));
        ConsoleHelper.AskToNamePlayers(game._game.Players);
        return game;
    }
    
    public void Start()
    {
        foreach (var item in _game.LogEntries)
        {
            Console.WriteLine(item.Text);
        }
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
        _game.StartRound();
        foreach (var item in _game.LogEntries.Where(l => l.Round == _game.CurrentRound))
        {
            Console.WriteLine(item.Text);
        }
        Console.WriteLine(_game.Prompt);
        while (_game.RoundActive)
        {
            var currentPlayer = _game.CurrentPlayer;
            if (currentPlayer == _game.Dealer)
            {
                DealerTurn(currentPlayer);
            }
            else
            {
                PlayerTurn(currentPlayer);
            }
            _game.MoveToNextPlayer();
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("The round is over! Calculating winnings!");

        foreach (var player in _game.Players.Except([_game.Dealer]))
        {
            ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
            Console.WriteLine($"{player.Name} had {string.Join('|', player.Hand.Select(c => c.Name))}");
            if (player.HandValue < _game.BlackJackScore && (_game.Dealer.HandValue > _game.BlackJackScore || _game.Dealer.HandValue < player.HandValue))
            {
                Console.WriteLine($"{player.Name} wins {player.CurrentBet * 2}");
                player.AddPoints(player.CurrentBet * 2);
            }
            else
            {
                Console.WriteLine($"Sadly {player.Name} lost this round...");
            }
        }
        Console.WriteLine("Here are the current scores:");
        foreach (var player in _game.Players.Except([_game.Dealer]).OrderBy(p => p.TotalPoints))
        {
            Console.WriteLine($"{player.Name} has ${player.TotalPoints}!");
        }
        Console.WriteLine();
    }
    private void DealerTurn(BlackJackPlayer dealer)
    {
        ConsoleHelper.SeperatorLine();
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(dealer));
        Console.WriteLine($"It's the Dealer's turn!");
        Console.WriteLine($"The dealer has {dealer.Hand.First().Name} and {dealer.Hand.Count -1} hidden cards.");
        var dealerRound = _game.PlayDealer();
        if (dealerRound == true)
            Console.WriteLine($"The dealer drew another card.");
        else if (dealerRound == false)
            Console.WriteLine($"The dealer is standing on their cards.");
    }
    private void PlayerTurn(BlackJackPlayer player)
    {
        ConsoleHelper.SeperatorLine();
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"It's {player.Name}'s turn!");
        Console.WriteLine($"They have {string.Join("", player.Hand.Select(c => c.Emoji))}({player.HandValue})");
        EvaluateHand(player);
        Console.WriteLine(_game.Prompt);
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
    private void Hit(BlackJackPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        var drawnCard = _game.Hit(player);
        Console.WriteLine($"{player.Name} drew a {drawnCard.Name}!");
        EvaluateHand(player);
    }
    private void DoubleDown(BlackJackPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"{player.Name} is doubling down!");
        var drawnCard = _game.DoubleDown(player);
        Console.WriteLine($"{player.Name} drew a {drawnCard.Name}!");
        EvaluateHand(player);
    }
    private void Stand(BlackJackPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"{player.Name} is standing on {player.HandValue}.");
        player.Stand();
    }
    private static void EvaluateHand(BlackJackPlayer player)
    {
        Console.WriteLine($"{player.Name} currently has {player.HandValue}.");
        if (player.HandValue == 21)
        {
            if (player.Hand.Count == 2)
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
