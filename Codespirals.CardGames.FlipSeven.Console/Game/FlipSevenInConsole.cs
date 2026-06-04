namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenInConsole
{
    private readonly FlipSevenGame _game;
    private FlipSevenInConsole(int playerCount)
    {
        _game = FlipSevenGame.SetUp(playerCount);
    }
    public static FlipSevenInConsole SetUp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Starting a game of Flip7");
        return new(ConsoleHelper.GetPlayerCount(3, 8));
    }

    public void Start()
    {
        ConsoleHelper.AskToNamePlayers(_game.Players);
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
            var currentPlayer = _game.CurrentPlayer;
            PlayerTurn(currentPlayer);
            _game.MoveToNextPlayer();
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("The round is over!");
        Console.WriteLine("Here are the current scores:");
        foreach (var player in _game.Players.OrderBy(p => p.TotalPoints))
        {
            Console.WriteLine($"{player.Name} has {player.TotalPoints}!");
        }
        Console.WriteLine();
    }

    private void PlayerTurn(FlipSevenPlayer player)
    {
        ConsoleHelper.SeperatorLine();
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        Console.WriteLine($"It's {player.Name}'s turn!");
        if (player.Hand.Count != 0)
        {
            Console.WriteLine(player.ToString());
        }
        Console.WriteLine($"What will you do? {nameof(Flip)} or {nameof(Bank)}?");
        var input = ConsoleHelper.ReadUntilAccepted([nameof(Flip), nameof(Bank)]);
        if (input.Equals(nameof(Flip), StringComparison.InvariantCultureIgnoreCase))
        {
            Flip(player);
            while (_game.ActionCardQueue.Any())
            {
                var action = _game.ActionCardQueue.First();
                UseTargetedCard(action.Player, action.ActionCard);
            }
        }
        else if (input.Equals(nameof(Bank), StringComparison.InvariantCultureIgnoreCase))
        {
            Bank(player);
        }
        Console.WriteLine();
    }
    private void Flip(FlipSevenPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        var drawnCard = _game.Flip();
        Console.WriteLine($"{player.Name} flipped a {drawnCard.Name}!");
        if (player.State == PlayerState.Busted)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Oh no... busted");
        }
    }
    private void Flip(FlipSevenPlayer player, int number)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        var drawnCards = _game.ForceFlip(player, number);
        foreach (var card in drawnCards)
        {
            Console.WriteLine($"{player.Name} flipped a {card.Name}!");
        }
        if (player.State == PlayerState.Busted)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Oh no... busted");
        }
    }
    private void Freeze(FlipSevenPlayer user, FlipSevenPlayer target)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(target));
        _game.Freeze(target);
        Console.WriteLine($"{target.Name} was frozen by {user.Name}");
    }

    private void Bank(FlipSevenPlayer player)
    {
        ConsoleHelper.SetColorForPlayer(_game.Players.IndexOf(player));
        if (player.HandPoints == 0)
            Console.WriteLine($"You have no points to bank... But ok.");
        else
            Console.WriteLine($"{player.Name} chose to bank their points. That's probably sensible.");
        player.Bank();
        Console.WriteLine($"{player.Name} now has {player.TotalPoints} Points!");
    }
    private void UseTargetedCard(FlipSevenPlayer user, FlipSevenCard card)
    {
        Console.WriteLine($"{user.Name} input the number to the right of the player you want to use this {card.Name} on!");
        Console.WriteLine($"Your targets are:");
        var i = 1;
        var validTargets = _game.GetValidTargets(card)?.ToList() ?? [];
        foreach (var option in validTargets)
        {
            Console.WriteLine($"For {option.Name} (Saved:{option.TotalPoints} | Hand:{option.HandPoints}) - Type: {i}");
            i++;
        }
        var selectedPlayerIndex = ConsoleHelper.ReadUntilInt(1, validTargets.Count);
        var target = validTargets[selectedPlayerIndex - 1];

        var result = _game.UseActionCard(target, card);

        if (result is null)
        {
            Console.WriteLine($"Error while trying to use {card.Name} on {target.Name}...");
        }
        else if (!result.Any())
        {
            Console.WriteLine($"{target.Name} was frozen.");
        }
        else
        {
            Console.WriteLine($"{target.Name} drew: {string.Join(" | ", result.Select(c => c.Name))}");
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
