using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc />
public class FlipSevenGame : IFlipSevenGame<FlipSevenGame, FlipSevenPlayer, FlipSevenDeck, FlipSevenCard>
{
    private readonly List<FlipSevenPlayer> _players = [];
    private FlipSevenPlayer _currentPlayer;
    private int _currentRound = 0;
    private List<LogEntry> _logEntries = [];
    private List<(FlipSevenPlayer, FlipSevenCard)> _actionCardQueue = [];

    /// <inheritdoc />
    public FlipSevenDeck Deck { get; } = FlipSevenDeckBuilder.CreateStandardDeck();
    /// <inheritdoc />
    public ReadOnlyCollection<FlipSevenPlayer> Players => _players.AsReadOnly();
    /// <inheritdoc />
    public FlipSevenPlayer CurrentPlayer => _currentPlayer;
    /// <inheritdoc />
    public int CurrentRound => _currentRound;
    /// <inheritdoc />
    public int WinningScore { get; set; } = 200;
    /// <inheritdoc />
    public int NumbersToFlip { get; set; } = 7;
    /// <inheritdoc />
    public int FlipNumberBonus { get; set; } = 15;
    /// <inheritdoc />
    public ReadOnlyCollection<(FlipSevenPlayer Player, FlipSevenCard ActionCard)> ActionCardQueue => _actionCardQueue.AsReadOnly();
    /// <inheritdoc />
    public bool RoundActive => !_players.All(p => p.IsOutForRound);
    /// <inheritdoc />
    public bool GameOver => !RoundActive && _players.Any(p => p.TotalPoints > WinningScore);

    /// <inheritdoc />
    public string Prompt { get; private set; } = "";
    /// <inheritdoc />
    public ReadOnlyCollection<LogEntry> LogEntries => _logEntries.AsReadOnly();

    /// <inheritdoc />
    private FlipSevenGame(int players, int numbersToFlip = 7, int flipNumberBonus = 15, int winningScore = 200, FlipSevenDeck? deck = null)
    {
        for (var i = 0; i < players; i++)
        {
            _players.Add(FlipSevenPlayer.GeneratePlayer(this, i+1));
        }
        _currentPlayer = _players.First();
        Deck = deck ?? FlipSevenDeckBuilder.CreateStandardDeck();
        NumbersToFlip = numbersToFlip;
        WinningScore = winningScore;
        Deck.Shuffle();
        Log("Starting a new game of Flip Seven.");
        Prompt = "Start playing!";
    }

    /// <inheritdoc />
    public static FlipSevenGame SetUp(int players)
        => new(players);
    /// <inheritdoc />
    public static FlipSevenGame SetUp(int players, int numbersToFlip = 7, int flipNumberBonus = 15, int winningScore = 200, FlipSevenDeck? deck = null)
        => new(players, numbersToFlip, flipNumberBonus, winningScore, deck);

    /// <inheritdoc />
    public FlipSevenPlayer GetCurrentPlayer() => _currentPlayer;

    /// <inheritdoc />
    public void StartRound()
    {
        if (GameOver)
            return;
        _currentRound++;
        Log($"Starting round {_currentRound}");
        foreach (var player in _players)
        {
            player.Reactivate();
        }
        _currentPlayer = Players[_currentRound - 1 % _players.Count];
        Log($"It's {_currentPlayer.Name}'s turn.");
        Prompt = $"{_currentPlayer.Name}: Choose an action!";
    }
    /// <inheritdoc />
    public int? Bank(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;
        Log($"{player.Name} is banking their {player.HandPoints} points.");
        return player.Bank();
    }
    /// <inheritdoc />
    public FlipSevenCard? Flip(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;

        var card = Deck.Draw();
        if (card is null)
            return null;

        Log($"{player.Name} flipped a {card.Name}.");
        // action cards need to be played immediately
        if (card.IsActionCard)
        {
            Prompt = $"{_currentPlayer.Name} Choose a player to give the card to!";
            _actionCardQueue.Add((player, card));
            return card;
        }

        player.AddCardToHand(card);

        // is number and player already has number
        if (card.CardType == CardType.Number && player.Hand.Any(c => c.CardType == CardType.Number && c.Value == card.Value))
        {
            Log($"Oh no, {player.Name} already has a {card.Name}.");
            var hasSecondChance = player.Hand.FirstOrDefault(c => c.CardType == CardType.SecondChance);
            if (hasSecondChance is not null)
            {
                Log($"Phew, {player.Name} had a {hasSecondChance.Name} to save them!");
                player.Discard(card);
                player.Discard(hasSecondChance);
            }
            else
            {
                Log($"{player.Name} got busted...");
                player.Bust();
            }
        }
        else if (player.NumberCardsInHand == NumbersToFlip)
        {
            Log($"Wow, {player.Name} managed to get {NumbersToFlip} Number cards!");
            EndRound();
        }
        return card;
    }
    /// <inheritdoc />
    public IEnumerable<FlipSevenPlayer> GetValidTargets()
        => _players.Where(p => !p.IsOutForRound);
    /// <inheritdoc />
    public IEnumerable<FlipSevenCard>? TryGivePlayerCard(FlipSevenPlayer target, FlipSevenCard card)
    {
        if (target.IsOutForRound || !card.IsActionCard)
            return null;

        if (_currentPlayer == target)
            Log($"{_currentPlayer.Name} is taking {card.Name} for themselves!");
        else
            Log($"{_currentPlayer.Name} is giving {card.Name} to {target.Name}!");
         
        switch (card.CardType)
        {
            case CardType.SecondChance:
                if (target.Hand.Any(c => c.CardType == CardType.SecondChance))
                {
                    Log($"{target.Name} already has a {card.Name}! It's not called \"third chance\"...");
                    Prompt = $"{_currentPlayer.Name} choose another player.";
                    return null;
                }
                target.AddCardToHand(card);
                _actionCardQueue.RemoveAt(0);
                return [];
            case CardType.Flip:
                var newCards = Flip(target, card.Value);
                _actionCardQueue.RemoveAt(0);
                return newCards;
            case CardType.Freeze:
                var gainedPoints = Freeze(target);
                if (gainedPoints is null)
                    return null;
                _actionCardQueue.RemoveAt(0);
                return [];
            default:
                return null;
        }
    }
    /// <inheritdoc />
    public IEnumerable<FlipSevenCard> Flip(FlipSevenPlayer target, int number)
    {
        Log($"{target.Name} has to flip {number}!");
        for (int i = 0; i < number; i++)
        {
            var card = Flip(target);
            if (card is null)
                yield break;
            yield return card;
        }
    }
    /// <inheritdoc />
    public int? Freeze(FlipSevenPlayer target)
    {
        if (target.IsOutForRound)
            return null;
        Log($"{target.Name} got frozen on {target.HandPoints}.");
        return target.Freeze();
    }
    /// <inheritdoc />
    public void MoveToNextPlayer()
    {
        if (_players.All(p => p.IsOutForRound) || GameOver)
        {
            EndRound();
            return;
        }

        var nextPlayerIndex = (_players.IndexOf(CurrentPlayer) + 1) % _players.Count;
        _currentPlayer = _players[nextPlayerIndex];

        if (_currentPlayer.IsOutForRound)
            MoveToNextPlayer();

        Log($"It's {_currentPlayer.Name}'s turn.");
        Prompt = $"{_currentPlayer.Name}: Choose an action!";
    }
    /// <inheritdoc />
    public void EndRound()
    {
        foreach (var player in _players.Where(p => !p.IsOutForRound))
            player.Bank();
        Log($"Round {CurrentRound} has ended.");
        Prompt = $"Pay the players their winnings.";
    }
    /// <inheritdoc />
    public (FlipSevenPlayer Player, int Winnings)[] CalculateCurrentPotentialPointGain()
    {
        (FlipSevenPlayer Player, int WinningMultiplier)[] results = [];
        foreach (var player in Players)
        {
            var winnings = -1;
            if (player.State == PlayerStates.Busted)
            {
                winnings = 0;
            }
            else
            {
                winnings = player.HandPoints;
            }
            results = results.Append((player, winnings)).ToArray();
        }
        return results;
    }
    /// <inheritdoc />
    public void PayOut()
    {
        if (Players.Any(p => !p.IsOutForRound))
            return;

        Log($"Adding everyone's points to their total:");
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            item.Player.AddPoints(item.Winnings);
            if (item.Winnings < 1)
                Log($"{item.Player.Name} had got nothing this round.");
            else if (item.Player.NumberCardsInHand == NumbersToFlip)
                Log($"{item.Player.Name} flipped {NumbersToFlip}! They bank {item.Winnings} which includes a {FlipNumberBonus} Bonus");
            else
                Log($"{item.Player.Name} banked {item.Winnings}.");
            Log($"They have {item.Player.TotalPoints} in total now.");
        }
        Prompt = $"Start the next round!";
    }
    /// <inheritdoc />
    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        var winner = _players.MaxBy(p => p.TotalPoints);
        Log($"{winner?.Name} wins!");
        return winner;
    }

    /// <inheritdoc/>
    public void Log(string text)
        => _logEntries.Add(new LogEntry(text, CurrentRound));
}
