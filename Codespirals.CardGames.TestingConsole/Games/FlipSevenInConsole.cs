using Codespirals.CardGames.FlipSeven;

namespace Codespirals.CardGames.TestingConsole.Games;
internal class FlipSevenInConsole
{
    private Game _game;
    private FlipSevenInConsole(int playerCount)
    {
        _game = Game.SetUp(playerCount);
    }
    public static FlipSevenInConsole SetUp()
    {
        Console.WriteLine("Starting a game of Flip 7");
        Console.WriteLine("How many players are playing?");
        var playerCount = ConsoleHelper.ReadUntilInt();
        return new(playerCount);
    }
    public void Start()
    {
        while (!_game.GameOver)
        {
            PlayRound();
        }
        End();
    }

    private void PlayRound()
    {
        Console.WriteLine($"Starting round {_game.CurrentRound}");
        _game.StartRound();
        while (_game.RoundActive)
        {
            var currentPlayer = _game.GetCurrentPlayer();
            PlayerTurn(currentPlayer);
            _game.MoveToNextPlayer();
        }
        Console.WriteLine("The round is over!");
        Console.WriteLine("Here are the current scores:");
        foreach (var player in _game.Players.OrderBy(p => p.Points))
        {
            Console.WriteLine($"{player.Name} has {player.Points}!");
        }
    }
    private void PlayerTurn(Player player)
    {
        Console.WriteLine($"It's {player.Name}'s turn!");
        Console.WriteLine($"What will you do? Flip or bank?");
        var input = ConsoleHelper.ReadUntilAccepted(["flip", "bank"]);
        if (input.Equals("flip", StringComparison.InvariantCultureIgnoreCase))
        {
            Flip(player);
        }
        else if (input.Equals("bank", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine($"{player.Name} chose to bank their points. That's probably sensible.");
            player.BankPoints(_game.Deck);
            Console.WriteLine($"{player.Name} has {player.Points}!");
        }
    }
    private void Flip(Player player)
    {
        var drawnCard = _game.Flip(player);
        Console.WriteLine($"{player.Name} drew a {drawnCard.Name}!");
        if (drawnCard.CardType is CardType.Freeze or CardType.FlipThree)
        {
            UseTargetedCard(drawnCard);
        }
    }
    private void UseTargetedCard(Card card)
    {
        Console.WriteLine($"Input the id of the player you want to use this {card.Name} on!");

        Console.WriteLine($"Your targets are:");
        var i = 1;
        foreach (var option in _game.ActivePlayers)
        {
            Console.WriteLine($"For {option.Name} ({option.Points}) - Type: {i}");
        }
        var selectedPlayerIndex = ConsoleHelper.ReadUntilInt(1, _game.ActivePlayers.Count);
        var target = _game.ActivePlayers[selectedPlayerIndex];

        if (card.CardType == CardType.FlipThree)
        {
            Console.WriteLine($"Do a flip!");
            for (int j = 0; j < 3; j++)
            {
                Flip(target);
            }
        }
        else if (card.CardType == CardType.Freeze)
        {
            target.Freeze(_game.Deck);
            Console.WriteLine($"Brr... Ice cold.");
        }
    }
    private void End()
    {
        var winner = _game.GetWinner();
        if (winner is null)
        {
            Console.WriteLine("No winner is determined yet... why did you end the game?");
            return;
        }
        Console.WriteLine($"Congratulations to {winner.Name}! You've won Flip7!");
    }
}
