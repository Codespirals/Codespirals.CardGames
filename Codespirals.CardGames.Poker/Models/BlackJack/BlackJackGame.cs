using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc cref="IBlackJackGame{TSelf, TPlayer, TDeck, TCard}"/>
public class BlackJackGame : IBlackJackGame<BlackJackGame, BlackJackPlayer, PokerDeck, PokerCard>
{
    private readonly List<BlackJackPlayer> _players = [];
    private bool _dealerCanCountCards;
    private List<LogEntry> _logEntries = [];

    /// <inheritdoc/>
    public PokerDeck Deck { get; }
    /// <inheritdoc/>
    public ReadOnlyCollection<BlackJackPlayer> Players => _players.ToList().AsReadOnly();
    /// <inheritdoc/>
    public BlackJackPlayer CurrentPlayer { get; private set; }
    /// <inheritdoc/>
    public int CurrentRound { get; private set; }
    /// <inheritdoc/>
    public BlackJackPlayer Dealer { get; }
    /// <inheritdoc/>
    public int BlackJackScore { get; }
    /// <inheritdoc/>
    public int BuyIn { get; private set; } = 1;
    /// <inheritdoc/>
    public int DrawAtStartOfRound { get; }
    /// <inheritdoc/>
    public int IncreaseStakeAfterRound { get; }
    /// <inheritdoc/>
    public bool RoundActive => _players.Any(p => !p.IsOutForRound);
    /// <inheritdoc/>
    public bool GameOver => Players.Except([Dealer]).All(p => p.TappedOut);

    /// <inheritdoc/>
    public string Prompt { get; private set; } = "";
    /// <inheritdoc/>
    public ReadOnlyCollection<LogEntry> LogEntries => _logEntries.AsReadOnly();

    /// <inheritdoc/>
    private BlackJackGame(int players, int minBet = 1, int startingCash = 100, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, int winningScore = 21, bool dealerCanCountCards = true, PokerDeck? deck = null)
    {
        BuyIn = minBet;
        IncreaseStakeAfterRound = automaticallyIncreaseStakeAfterRound;
        BlackJackScore = winningScore;
        DrawAtStartOfRound = drawAtStartOfRound;

        Dealer = PokerPlayerGenerator.GenerateBlackJackDealer();
        _dealerCanCountCards = dealerCanCountCards;
        _players.Add(Dealer);
        CurrentPlayer = Dealer;

        for (var i = 0; i < players; i++)
        {
            _players.Add(PokerPlayerGenerator.GenerateBlackJackPlayer(i, startingCash));
        }

        Deck = deck ?? PokerDeckBuilder.CreateBlackJackDeck();
        Deck.Shuffle();

        CurrentRound = 0;

        Log("Starting a new game of Blackjack.");
        Log($"The starting buyin is {BuyIn}");
        Prompt = "Start playing!";
    }

    /// <inheritdoc/>
    public static BlackJackGame SetUp(int players)
        => new(players);
    /// <inheritdoc/>
    public static BlackJackGame SetUp(int players, int minBet = 1, int startingCash = 100, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, int winningScore = 21, bool dealerCanCountCards = true, PokerDeck? deck = null)
        => new(players:players, minBet:minBet, startingCash:startingCash, automaticallyIncreaseStakeAfterRound:automaticallyIncreaseStakeAfterRound, drawAtStartOfRound:drawAtStartOfRound, winningScore:winningScore, dealerCanCountCards:dealerCanCountCards, deck:deck);

    /// <inheritdoc/>
    public void StartRound()
    {
        if (GameOver)
            return;
        CurrentRound++;
        Log($"Starting round {CurrentRound}");
        foreach (var player in _players)
        {
            if (player.TappedOut)
                continue;
            Deck.PutOnDiscardPile(player.DiscardAll());
            player.Reactivate();
            player.Bet(BuyIn);
            for (int i = 0; i < DrawAtStartOfRound; i++)
            {
                var card = Deck.Draw();
                if (card is not null)
                    player.AddCardToHand(card);
            }
        }
        CurrentPlayer = Players[1];
    }

    #region Choices
    /// <inheritdoc/>
    public PokerCard? Hit()
    {
        var card = Deck.Draw();
        if (card is null)
            return null;

        CurrentPlayer.AddCardToHand(card);
        Log($"{CurrentPlayer.Name} drew a {card.Name}.", GetPlayerId(CurrentPlayer));

        if (CalculateHandValue(CurrentPlayer) > BlackJackScore)
            CurrentPlayer.Bust();

        MoveToNextPlayer();
        return card;
    }

    /// <inheritdoc/>
    public PokerCard? DoubleDown()
    {
        var card = Deck.Draw();
        if (card is null)
            return null;

        CurrentPlayer.Bet(Math.Clamp(CurrentPlayer.CurrentBet, 0, CurrentPlayer.TotalPoints));
        CurrentPlayer.AddCardToHand(card);
        CurrentPlayer.Stand();

        Log($"{CurrentPlayer.Name} doubled down! Their bet is now {CurrentPlayer.CurrentBet}.", GetPlayerId(CurrentPlayer));
        Log($"{CurrentPlayer.Name} drew a {card.Name}.", GetPlayerId(CurrentPlayer));
        MoveToNextPlayer();
        return card;
    }

    /// <inheritdoc/>
    public void Stand()
    {
        CurrentPlayer.Stand();
        Log($"{CurrentPlayer.Name} is standing on {CalculateHandValue(CurrentPlayer)}.", GetPlayerId(CurrentPlayer));
        MoveToNextPlayer();
    }
    #endregion

    /// <inheritdoc/>
    public void MoveToNextPlayer()
    {
        if (_players.All(p => p.IsOutForRound) || GameOver)
        {
            EndRound();
            return;
        }

        var nextPlayerIndex = (_players.IndexOf(CurrentPlayer) + 1) % _players.Count;
        CurrentPlayer = _players[nextPlayerIndex];

        if (CurrentPlayer.IsOutForRound)
        {
            MoveToNextPlayer();
            return;
        }
        if (CurrentPlayer == Dealer)
        {
            PlayDealer();
            MoveToNextPlayer();
            return;
        }
        Log($"It's {CurrentPlayer.Name}'s turn.", GetPlayerId(CurrentPlayer));
        Prompt = $"{CurrentPlayer.Name}: Choose an action!";
    }

    /// <inheritdoc/>
    public bool? PlayDealer()
    {
        if (Dealer.IsOutForRound)
            return null;

        var id = GetPlayerId(Dealer);
        Log($"It's the dealer's turn.", id);
        var averageValueOfCardPool = _dealerCanCountCards ? Deck.CardPool.Average(c => c.Value) : 7.3;
        if (CalculateHandValue(Dealer) <= BlackJackScore - Math.Ceiling(averageValueOfCardPool / 2))
        {
            var newCard = Deck.Draw();
            if (newCard is null)
                return null;
            Dealer.AddCardToHand(newCard);
            Log($"The dealer drew a card.", id);
            return true;
        }
        else
        {
            Dealer.Stand();
            Log($"The dealer is standing on their cards.", id);
            return false;
        }
    }

    /// <inheritdoc/>
    public void EndRound()
    {
        foreach (var player in _players)
            player.DeactivateForRound();
        Log($"Round {CurrentRound} has ended.");
        Prompt = $"Pay the players their winnings.";
    }

    /// <inheritdoc/>
    public int CalculateHandValue(BlackJackPlayer player)
    {
        var value = player.Hand.Sum(c => c.Value);
        foreach (var ace in player.Hand.Where(c => c.Value == 11))
        {
            if (value <= BlackJackScore)
                break;
            value -= 10;
        }
        return value;
    }

    /// <inheritdoc/>
    public IEnumerable<(BlackJackPlayer Player, int Winnings)> CalculateCurrentPotentialPointGain()
    {
        (BlackJackPlayer Player, int Winnings)[] results = [];
        foreach (var player in _players.Except([Dealer]))
        {
            var winningMultiplier = -1;
            var handValue = CalculateHandValue(player);

            if (player.IsBusted)
            {
                winningMultiplier = 0;
            }
            else if (handValue == BlackJackScore && player.Hand.Count == 2)
            {
                // blackjack
                winningMultiplier = 3;
            }
            else if ((Dealer.IsBusted && !player.IsBusted)
                || (!Dealer.IsBusted && !player.IsBusted && handValue > CalculateHandValue(Dealer)))
            {
                // win
                winningMultiplier = 2;
            }
            else if (!player.IsBusted && handValue == CalculateHandValue(Dealer))
            {
                // draw
                winningMultiplier = 1;
            }
            results = results.Append((player, winningMultiplier * player.CurrentBet)).ToArray();
        }

        return results;
    }

    /// <inheritdoc/>
    public void PayOut()
    {
        if (Players.Any(p => !p.IsOutForRound))
            return;

        Log(new string('=', 20));

        Log($"Paying out to all players:");
        Log($"The dealer ended the round with {CalculateHandValue(Dealer)}", GetPlayerId(Dealer));
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            var id = GetPlayerId(item.Player);
            var handValue = CalculateHandValue(item.Player);
            if (item.Winnings < 1)
                Log($"{item.Player.Name} had {handValue} and lost...", id);
            else if (item.Winnings == BuyIn)
                Log($"{item.Player.Name} had {handValue} and drew with the dealer! +{item.Winnings}", id);
            else if (handValue == BlackJackScore && item.Player.Hand.Count == 2)
                Log($"{item.Player.Name} got a blackjack! +{item.Winnings}", id);
            else
                Log($"{item.Player.Name} had {handValue} and won! +{item.Winnings}", id);
            item.Player.AddPoints(item.Winnings);
        }

        if (IncreaseStakeAfterRound > 0)
            RaiseTheStakes(IncreaseStakeAfterRound);
        Prompt = $"Start the next round!";
    }

    /// <inheritdoc/>
    public void RaiseTheStakes(int amount)
    {
        BuyIn += amount;
        Log($"The stakes have risen to {BuyIn}!");
    }

    /// <inheritdoc/>
    public void Log(string text, int? actorId = null)
        => _logEntries.Add(new LogEntry(text, CurrentRound, actorId ?? -1));

    private int GetPlayerId(BlackJackPlayer? player)
        => player is not null ? Players.IndexOf(player) : -1;
}
