using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc />
public class FlipSevenGame : IFlipSevenGame<FlipSevenGame, FlipSevenPlayer, FlipSevenDeck, FlipSevenCard>
{
    private readonly List<FlipSevenPlayer> _players = [];
    private FlipSevenPlayer _currentPlayer;
    private List<LogEntry> _logEntries = [];
    private List<(FlipSevenPlayer Player, FlipSevenCard ActionCard)> _actionCardQueue = [];

    /// <inheritdoc />
    public FlipSevenDeck Deck { get; }
    /// <inheritdoc />
    public ReadOnlyCollection<FlipSevenPlayer> Players => _players.AsReadOnly();
    /// <inheritdoc />
    public FlipSevenPlayer CurrentPlayer => GetCurrentPlayerOrActionQueuePlayer();
    /// <inheritdoc />
    public int CurrentRound { get; private set; }
    /// <inheritdoc />
    public int WinningScore { get; }
    /// <inheritdoc />
    public int NumbersToFlip { get; }
    /// <inheritdoc />
    public int FlipNumberBonus { get; }
    /// <inheritdoc />
    public bool PlayersCanHaveMultipleSecondChances { get; }
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
        // add deck
        Deck = deck ?? FlipSevenDeckBuilder.CreateStandardDeck();

        // set... settings
        var highestCardValue = Deck.HighestNumberCard;
        WinningScore = Math.Clamp(winningScore, highestCardValue, short.MaxValue);
        NumbersToFlip = Math.Clamp(numbersToFlip, 2, highestCardValue);
        FlipNumberBonus = Math.Clamp(flipNumberBonus, 0, WinningScore);

        // add players
        var maxPlayers = (int)Math.Floor(Deck.TotalNumberCards / (decimal)NumbersToFlip);
        for (var i = 0; i < Math.Clamp(players, 2, maxPlayers); i++)
        {
            _players.Add(FlipSevenPlayerGenerator.GeneratePlayer(i+1));
        }
        _currentPlayer = _players.First();

        // finish
        Deck.Shuffle();
        CurrentRound = 0;
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
    public void StartRound()
    {
        if (GameOver)
            return;
        CurrentRound++;
        Log($"Starting round {CurrentRound}");
        foreach (var player in _players)
        {
            this.Deck.PutOnDiscardPile(player.DiscardAll());
            player.Reactivate();
        }
        var nextPlayerIndex = (CurrentRound - 1) % _players.Count;
        _currentPlayer = _players[nextPlayerIndex];
        Log($"It's {_currentPlayer.Name}'s turn.", GetPlayerIndex(_currentPlayer));
        Prompt = $"{_currentPlayer.Name}: Choose an action!";
    }

    #region Player actions
    /// <inheritdoc />
    public void Bank()
    {
        if (_currentPlayer.IsOutForRound)
            return;
        Log($"{_currentPlayer.Name} is banking their {CalculateHandValueForPlayer(_currentPlayer)} points.", GetPlayerIndex(_currentPlayer));
        _currentPlayer.Bank();
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
    public IEnumerable<FlipSevenCard>? UseActionCard(FlipSevenPlayer target, FlipSevenCard? card)
    {
        if (target.IsOutForRound || card is null || !card.IsActionCard)
            return null;

        if (_currentPlayer == target)
            Log($"{_currentPlayer.Name} is taking the {card.Name} themselves!", GetPlayerIndex(_currentPlayer));
        else
            Log($"{_currentPlayer.Name} is giving the {card.Name} to {target.Name}!", GetPlayerIndex(_currentPlayer));

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
                Deck.PutOnDiscardPile(card);
                break;
            case CardType.Freeze:
                Freeze(target);
                result = [];
                Deck.PutOnDiscardPile(card);
                break;
            default:
                break;
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
            Log($"{target.Name} already has a {secondChance.Name}! It's not called \"third chance\"...", GetPlayerIndex(_currentPlayer));
            Prompt = $"{_currentPlayer.Name} choose another player.";
            return;
        }
        Log($"The {secondChance.Name} was given to {target.Name}.", GetPlayerIndex(target));
        target.AddCardToHand(secondChance);
    }
    /// <inheritdoc />
    public IEnumerable<FlipSevenCard> ForceFlip(FlipSevenPlayer target, int number)
    {
        Log($"{target.Name} has to flip {number}!", GetPlayerIndex(target));
        List<FlipSevenCard> result = [];
        for (int i = 0; i < number; i++)
        {
            var card = Flip(target);
            if (target.IsBusted)
                break;
        }
        return result;
    }
    /// <inheritdoc />
    public void Freeze(FlipSevenPlayer target)
    {
        if (target.IsOutForRound)
            return;
        Log($"{target.Name} got frozen on {CalculateHandValueForPlayer(target)}.", GetPlayerIndex(target));
        target.Freeze();
    }
    #endregion

    #region auto methods
    /// <inheritdoc />
    public void MoveToNextPlayer()
    {
        if (_players.All(p => p.IsOutForRound) || GameOver)
        {
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

        Log($"It's {_currentPlayer.Name}'s turn.", GetPlayerIndex(_currentPlayer));
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
    public void PayOut()
    {
        if (Players.Any(p => !p.IsOutForRound))
            return;

        Log(new string('=', 20));
        Prompt = $"Start the next round!";

        if (Players.All(p => CalculateHandValueForPlayer(p) == 0))
        {
            Log($"Nobody gained any points this round...");
            return;
        }

        Log($"Adding everyone's points to their total:");
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            item.Player.AddPoints(item.Winnings);
            // technically it's not currently possible to lose points, but hey, if you want to add negative "add" cards, that's possible
            if (item.Winnings < 0)
                Log($"{item.Player.Name} LOST {Math.Abs(item.Winnings)} points... Ouch.", GetPlayerIndex(item.Player));
            else if (item.Winnings == 0)
                Log($"{item.Player.Name} gained no points this round.", GetPlayerIndex(item.Player));
            else if (item.Player.NumberCardsInHand == NumbersToFlip)
                Log($"{item.Player.Name} flipped {NumbersToFlip}! They bank {item.Winnings} which includes {FlipNumberBonus} Bonus points!", GetPlayerIndex(item.Player));
            else
                Log($"{item.Player.Name} banked {item.Winnings}.", GetPlayerIndex(item.Player));
            Log($"{item.Player.Name} has {item.Player.TotalPoints} in total.", GetPlayerIndex(item.Player));
        }
    }
    /// <inheritdoc />
    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        Log($"GAME OVER");
        var winner = _players.MaxBy(p => p.TotalPoints);
        if (winner is null)
            return null;

        Prompt = $"{winner?.Name} wins!";
        Log(Prompt, GetPlayerIndex(winner));
        return winner;
    }
    #endregion

    #region unlogged methods
    /// <inheritdoc />
    public int CalculateHandValueForPlayer(FlipSevenPlayer player)
    {
        if (player.IsBusted)
            return 0;

        var points = 0;
        foreach (var card in player.Hand.OrderBy(c => c.CardType))
        {
            switch (card.CardType)
            {
                case CardType.Number:
                    points += card.Value;
                    break;
                case CardType.Multiplier:
                    points *= 2;
                    break;
                case CardType.BonusAdd:
                    points += card.Value;
                    break;
                default:
                    break;
            }
        }
        return points;
    }
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
    public IEnumerable<(FlipSevenPlayer Player, int Winnings)> CalculateCurrentPotentialPointGain()
    {
        (FlipSevenPlayer Player, int WinningMultiplier)[] results = [];
        foreach (var player in Players)
        {
            var winnings = -1;
            if (player.State == PlayerState.Busted)
                winnings = 0;
            else if (player.NumberCardsInHand == NumbersToFlip)
                winnings = CalculateHandValueForPlayer(player) + FlipNumberBonus;
            else
                winnings = CalculateHandValueForPlayer(player);

            results = results.Append((player, winnings)).ToArray();
        }
        return results;
    }

    /// <inheritdoc/>
    public void Log(string text, int? actorId = null)
        => _logEntries.Add(new LogEntry(text, CurrentRound, actorId ?? -1));
    #endregion

    /// <inheritdoc/>
    public int GetPlayerIndex(FlipSevenPlayer? player)
        => player is not null ? Players.IndexOf(player) : -1;

    private FlipSevenCard? Flip(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;

        var drawnCard = Deck.Draw();
        if (drawnCard is null)
            return null;

        var id = GetPlayerIndex(player);

        Log($"{player.Name} flipped a {drawnCard.Name}.", id);
        // action cards need to be played immediately 
        if (drawnCard.IsActionCard)
        {
            var targets = GetValidTargets(drawnCard);
            if (targets is null || !targets.Any())
            {
                Log($"There are no valid players to give a {drawnCard.Name} to, so it has to be discarded.", id);
                Deck.PutOnDiscardPile(drawnCard);
                return drawnCard;
            }
            _actionCardQueue.Add((player, drawnCard));
            return drawnCard;
        }

        // is number and player already has number
        if (drawnCard.CardType == CardType.Number && player.Hand.Any(c => c.CardType == CardType.Number && c.Value == drawnCard.Value))
        {
            Log($"Oh no, {player.Name} already has a {drawnCard.Name}!", id);
            var hasSecondChance = player.Hand.FirstOrDefault(c => c.CardType == CardType.SecondChance);
            if (hasSecondChance is not null)
            {
                Log($"Phew, {player.Name} had a {hasSecondChance.Name} to save them!", id);
                var successfullDiscard = player.Discard(hasSecondChance);
                if (successfullDiscard is null)
                    return null;
                Deck.PutOnDiscardPile(successfullDiscard);
                Deck.PutOnDiscardPile(drawnCard);
                return drawnCard;
            }
            else
            {
                Log($"{player.Name} got busted...", id);
                player.Bust();
            }
        }

        player.AddCardToHand(drawnCard);

        if (!player.IsOutForRound && player.NumberCardsInHand == NumbersToFlip)
        {
            Log($"Wow, {player.Name} managed to flip {NumbersToFlip}!", id);
            EndRound();
        }
        return drawnCard;
    }

    private FlipSevenPlayer GetCurrentPlayerOrActionQueuePlayer()
    {
        var playerInActionQueue = ActionCardQueue.Any() ? ActionCardQueue.First().Player : null;
        while (playerInActionQueue is not null && playerInActionQueue.IsOutForRound)
        {
            _actionCardQueue.RemoveAt(0);
            playerInActionQueue = ActionCardQueue.Any() ? ActionCardQueue.First().Player : null;
        }
        if (playerInActionQueue is null)
        {
            Prompt = $"{_currentPlayer.Name}: Choose an action!";
            return _currentPlayer;
        }
        else
        {
            Prompt = $"{playerInActionQueue.Name} Choose a player to give the card to!";
            return playerInActionQueue;
        }
    }
}
