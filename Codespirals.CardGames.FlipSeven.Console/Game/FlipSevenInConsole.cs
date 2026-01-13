namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenInConsole
{
    private readonly Game _game;
    private FlipSevenInConsole(int playerCount)
    {
        _game = Game.SetUp(playerCount);
    }
    public static FlipSevenInConsole SetUp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Starting a game of Flip7");
        Console.WriteLine("How many players are playing?");
        Console.WriteLine("Enter a number between 2 and 8:");
        var playerCount = ConsoleHelper.ReadUntilInt(2, 8);
        return new(playerCount);
    }

    public void Start()
    {
        NamePlayers();
        while (!_game.GameOver)
        {
            PlayRound();
        }
        End();
    }
    private void NamePlayers()
    {
        Console.WriteLine($"Do you want to name the players?");
        var answer = ConsoleHelper.ReadUntilAccepted(["y", "yes", "n", "no"]);
        if (answer.StartsWith('n'))
            return;
        foreach (var player in _game.Players)
        {
            ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
            var oldName = player.Name;
            Console.WriteLine($"Set {oldName}'s new name!");
            player.Name = ConsoleHelper.ReadUntilAccepted();
            Console.WriteLine($"{oldName} is now {player.Name}!");
        }
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
        Console.WriteLine("Here are the current scores:");
        foreach (var player in _game.Players.OrderBy(p => p.BankedPoints))
        {
            Console.WriteLine($"{player.Name} has {player.BankedPoints}!");
        }
        Console.WriteLine();
    }

    private void PlayerTurn(Player player)
    {
        ConsoleHelper.SeperatorLine();
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"It's {player.Name}'s turn!");
        if (player.Hand.Count != 0)
        {
            Console.WriteLine(player.ToString());
        }
        Console.WriteLine($"What will you do? Flip or bank?");
        var input = ConsoleHelper.ReadUntilAccepted(["flip", "bank"]);
        if (input.Equals("flip", StringComparison.InvariantCultureIgnoreCase))
        {
            Flip(player);
        }
        else if (input.Equals("bank", StringComparison.InvariantCultureIgnoreCase))
        {
            Bank(player);
        }
        Console.WriteLine();
    }
    private void Flip(Player player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        var drawnCard = _game.Flip(player);
        Console.WriteLine($"{player.Name} flipped a {drawnCard.Name}!");
        if (drawnCard.CardType is CardType.Freeze or CardType.Flip)
        {
            UseTargetedCard(player, drawnCard);
        }
        if (player.State == PlayerStates.Busted)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Oh no... busted");
        }
    }
    private void Freeze(Player user, Player target)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(target));
        target.Freeze();
        Console.WriteLine($"{target.Name} was frozen by {user.Name}");
    }

    private void Bank(Player player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        if (player.HandPoints == 0)
            Console.WriteLine($"You have no points to bank... But ok.");
        else
            Console.WriteLine($"{player.Name} chose to bank their points. That's probably sensible.");
        player.BankPoints();
        Console.WriteLine($"{player.Name} now has {player.BankedPoints} Points!");
    }
    private void UseTargetedCard(Player user, Card card)
    {
        Console.WriteLine($"{user.Name} input the number to the right of the player you want to use this {card.Name} on!");
        Console.WriteLine($"Your targets are:");
        var i = 1;
        foreach (var option in _game.ActivePlayers)
        {
            Console.WriteLine($"For {option.Name} (Saved:{option.BankedPoints} | Hand:{option.HandPoints}) - Type: {i}");
            i++;
        }
        var selectedPlayerIndex = ConsoleHelper.ReadUntilInt(1, _game.ActivePlayers.Count);
        var target = _game.ActivePlayers[selectedPlayerIndex - 1];

        if (card.CardType == CardType.Flip)
        {
            if (!user.Equals(target))
                Console.WriteLine($"{user.Name} is forcing {target.Name} to do a flip!");
            else
                Console.WriteLine($"{user.Name} is flipping out!");

            for (var j = 0; j < card.Value; j++)
            {
                Flip(target);
            }
        }
        else if (card.CardType == CardType.Freeze)
        {
            Freeze(user, target);
        }
    }
    private void End()
    {
        Console.ForegroundColor = ConsoleColor.White;
        var winner = _game.GetWinner();
        if (winner is null)
        {
            Console.WriteLine("No winner is determined yet... why did you end the game?");
            return;
        }
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(winner));
        Console.WriteLine($"Congratulations to {winner.Name}! You've won Flip7!");
    }
}
