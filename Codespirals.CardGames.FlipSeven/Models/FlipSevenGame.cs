using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc />
public class FlipSevenGame : IFlipSevenGame<FlipSevenGame, FlipSevenPlayer, FlipSevenDeck, FlipSevenCard>
{
    private readonly List<FlipSevenPlayer> _players = [];
    private FlipSevenPlayer _currentPlayer;
    private int _currentRound = 0;
    private List<LogEntry> _logEntries = [];
    private List<(FlipSevenPlayer Player, FlipSevenCard ActionCard)> _actionCardQueue = [];

    /// <inheritdoc />
    public FlipSevenDeck Deck { get; } = FlipSevenDeckBuilder.CreateStandardDeck();
    /// <inheritdoc />
    public ReadOnlyCollection<FlipSevenPlayer> Players => _players.AsReadOnly();
    /// <inheritdoc />
    public FlipSevenPlayer CurrentPlayer => ActionCardQueue.Any() ? ActionCardQueue.First().Player : _currentPlayer;
    /// <inheritdoc />
    public int CurrentRound => _currentRound;
    /// <inheritdoc />
    public int WinningScore { get; set; } = 200;
    /// <inheritdoc />
    public int NumbersToFlip { get; set; } = 7;
    /// <inheritdoc />
    public int FlipNumberBonus { get; set; } = 15;
    /// <inheritdoc />
    public bool PlayersCanHaveMultipleSecondChances { get; set; }
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

    #region Player actions
    /// <inheritdoc />
    public void Bank()
    {
        if (_currentPlayer.IsOutForRound)
            return;
        Log($"{_currentPlayer.Name} is banking their {_currentPlayer.HandPoints} points.");
        MoveToNextPlayer();
        return;
    }
    /// <inheritdoc />
    public FlipSevenCard? Flip()
    {
        var flippedCard = Flip(_currentPlayer);
        MoveToNextPlayer();
        return flippedCard;
    }

    /// <inheritdoc />
    public IEnumerable<FlipSevenCard>? UseActionCard(FlipSevenPlayer target, FlipSevenCard card)
    {
        if (target.IsOutForRound || !card.IsActionCard)
            return null;

        if (_currentPlayer == target)
            Log($"{_currentPlayer.Name} is taking {card.Name} for themselves!");
        else
            Log($"{_currentPlayer.Name} is giving {card.Name} to {target.Name}!");

        IEnumerable<FlipSevenCard>? result = null;
        switch (card.CardType)
        {
            case CardType.SecondChance:
                GiveSecondChance(target, card);
                result = [];
                break;
            case CardType.Flip:
                var newCards = ForceFlip(target, card.Value);
                result = newCards;
                break;
            case CardType.Freeze:
                Freeze(target);
                result = [];
                break;
            default:
                return null;
        }

        _actionCardQueue.RemoveAt(0);
        if (!ActionCardQueue.Any())
            MoveToNextPlayer();
        return result;
    }
    #endregion

    #region Action cards
    /// <inheritdoc />
    public void GiveSecondChance(FlipSevenPlayer target, FlipSevenCard secondChance)
    {
        if (target.Hand.Any(c => c.CardType == CardType.SecondChance) && !PlayersCanHaveMultipleSecondChances)
        {
            Log($"{target.Name} already has a {secondChance.Name}! It's not called \"third chance\"...");
            Prompt = $"{_currentPlayer.Name} choose another player.";
            return;
        }
        Log($"The {secondChance.Name} was given to {target.Name}.");
        target.AddCardToHand(secondChance);
    }
    /// <inheritdoc />
    public IEnumerable<FlipSevenCard> ForceFlip(FlipSevenPlayer target, int number)
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
    public void Freeze(FlipSevenPlayer target)
    {
        if (target.IsOutForRound)
            return;
        Log($"{target.Name} got frozen on {target.HandPoints}.");
        target.Freeze();
    }
    #endregion

    /// <inheritdoc />
    public IEnumerable<FlipSevenPlayer>? GetValidTargets(FlipSevenCard? card)
    {
        if (card is null || !card.IsActionCard)
            return null;
        if (!PlayersCanHaveMultipleSecondChances && card.CardType is CardType.SecondChance)
            return _players.Where(p => !p.IsOutForRound && !p.Hand.Any(c => c.CardType == CardType.SecondChance));
        return _players.Where(p => !p.IsOutForRound);
    }

    /// <inheritdoc />
    public void MoveToNextPlayer()
    {
        if (_players.All(p => p.IsOutForRound) || GameOver)
        {
            EndRound();
            return;
        }
        if (_currentPlayer.NumberCardsInHand == NumbersToFlip && !_currentPlayer.IsOutForRound)
        {
            Log($"Wow, {_currentPlayer.Name} managed to get {NumbersToFlip} Number cards!");
            EndRound();
            return;
        }
        if (ActionCardQueue.Any())
        {
            Prompt = $"There are active action cards that have to be played!";
            return;
        }

        var nextPlayerIndex = (_players.IndexOf(CurrentPlayer) + 1) % _players.Count;
        _currentPlayer = _players[nextPlayerIndex];

        if (_currentPlayer.IsOutForRound)
        {
            MoveToNextPlayer();
            return;
        }

        Log($"It's {_currentPlayer.Name}'s turn.");
        Prompt = $"{_currentPlayer.Name}: Choose an action!";
    }

    /// <inheritdoc />
    public void EndRound()
    {
        foreach (var player in _players.Where(p => !p.IsOutForRound))
            player.Bank();
        foreach (var queueItem in _actionCardQueue)
            Deck.PutOnDiscardPile(queueItem.ActionCard);
        _actionCardQueue = [];
        Log($"Round {CurrentRound} has ended.");
        PayOut();
    }

    /// <inheritdoc />
    public IEnumerable<(FlipSevenPlayer Player, int Winnings)> CalculateCurrentPotentialPointGain()
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
        foreach (var item in CalculateCurrentPotentialPointGain().OrderBy(p => p.Winnings))
        {
            item.Player.AddPoints(item.Winnings);
            if (item.Winnings < 1)
                Log($"{item.Player.Name} had got nothing this round.");
            else if (item.Player.NumberCardsInHand == NumbersToFlip)
                Log($"{item.Player.Name} flipped {NumbersToFlip}! They bank {item.Winnings} which includes {FlipNumberBonus} Bonus points!");
            else
                Log($"{item.Player.Name} banked {item.Winnings}.");
            Log($"{item.Player.Name} has {item.Player.TotalPoints} in total.");
        }
        Prompt = $"Start the next round!";
    }
    /// <inheritdoc />
    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        Log($"GAME OVER");
        var winner = _players.MaxBy(p => p.TotalPoints);
        Log($"{winner?.Name} wins!");
        return winner;
    }

    /// <inheritdoc/>
    public void Log(string text)
        => _logEntries.Add(new LogEntry(text, CurrentRound));

    private FlipSevenCard? Flip(FlipSevenPlayer player)
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
            var targets = GetValidTargets(card);
            if (targets is null || !targets.Any())
            {
                Log($"There are no valid players to give a {card.Name} to, so it has to be discarded.");
                Deck.PutOnDiscardPile(card);
                return card;
            }
            Prompt = $"{player.Name} Choose a player to give the card to!";
            _actionCardQueue.Add((player, card));
            return card;
        }

        // is number and player already has number
        if (card.CardType == CardType.Number && player.Hand.Any(c => c.CardType == CardType.Number && c.Value == card.Value))
        {
            Log($"Oh no, {player.Name} already has a {card.Name}.");
            var hasSecondChance = player.Hand.FirstOrDefault(c => c.CardType == CardType.SecondChance);
            if (hasSecondChance is not null)
            {
                Log($"Phew, {player.Name} had a {hasSecondChance.Name} to save them!");
                player.Discard(hasSecondChance);
            }
            else
            {
                Log($"{player.Name} got busted...");
                player.Bust();
            }
        }

        player.AddCardToHand(card);

        return card;
    }
}
