using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc cref="IBlackJackGame{TSelf, TPlayer, TDeck, TCard}"/>
public class BlackJackGame : IBlackJackGame<BlackJackGame, BlackJackPlayer, PokerDeck, PokerCard>
{
    private readonly List<BlackJackPlayer> _players = [];
    private BlackJackPlayer _currentPlayer;
    private int _currentRound = 0;
    private int _automaticallyIncreaseStakeAfterRound = 0;
    private bool _dealerCanCountCards;
    private List<LogEntry> _logEntries = [];

    /// <inheritdoc/>
    public PokerDeck Deck { get; }
    /// <inheritdoc/>
    public ReadOnlyCollection<BlackJackPlayer> Players => _players.ToList().AsReadOnly();
    /// <inheritdoc/>
    public BlackJackPlayer CurrentPlayer => _currentPlayer;
    /// <inheritdoc/>
    public int CurrentRound => _currentRound;
    /// <inheritdoc/>
    public BlackJackPlayer Dealer { get; }
    /// <inheritdoc/>
    public int BlackJackScore { get; private set; } = 21;
    /// <inheritdoc/>
    public int BuyIn { get; private set; }
    /// <inheritdoc/>
    public int DrawAtStartOfRound { get; private set; } = 2;
    /// <inheritdoc/>
    public bool RoundActive => _players.Any(p => !p.IsOutForRound);
    /// <inheritdoc/>
    public bool GameOver => Players.Except([Dealer]).All(p => p.TappedOut);

    /// <inheritdoc/>
    public string Prompt { get; private set; } = "";
    /// <inheritdoc/>
    public ReadOnlyCollection<LogEntry> LogEntries => _logEntries.AsReadOnly();

    /// <inheritdoc/>
    public BlackJackGame(int players, int minBet = 1, int blackJackScore = 21, int startingCash = 100, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, bool dealerCanCountCards = false, PokerDeck? deck = null)
    {
        Dealer = BlackJackPlayer.GeneratePlayer(this, "Dealer", Int32.MaxValue);
        _players.Add(Dealer);
        _currentPlayer = Dealer;
        _automaticallyIncreaseStakeAfterRound = automaticallyIncreaseStakeAfterRound;
        for (var i = 0; i < players; i++)
        {
            _players.Add(BlackJackPlayer.GeneratePlayer(this, i, startingCash));
        }
        BuyIn = minBet;
        BlackJackScore = blackJackScore;
        DrawAtStartOfRound = drawAtStartOfRound;
        Deck = deck ?? PokerDeckBuilder.CreateBlackJackDeck();
        Deck.Shuffle();
        _dealerCanCountCards = dealerCanCountCards;
        Log("Starting a new game of Blackjack.");
        Log($"The starting buyin is {BuyIn}");
        Prompt = "Start playing!";
    }

    /// <inheritdoc/>
    public static BlackJackGame SetUp(int players) => SetUp(players, 100, 1, 21, 1, 2, true, null);
    /// <inheritdoc/>
    public static BlackJackGame SetUp(int players, int startingCash, int minBet = 1, int winningScore = 21, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, bool dealerCanCountCards = false, PokerDeck? deck = null)
        => new(players, startingCash, minBet, winningScore, automaticallyIncreaseStakeAfterRound, drawAtStartOfRound, dealerCanCountCards, deck);

    /// <inheritdoc/>
    public void StartRound()
    {
        if (GameOver)
            return;
        _currentRound++;
        Log($"Starting round {_currentRound}");
        foreach (var player in _players)
        {
            if (player.TappedOut)
                continue;
            player.Reactivate();
            player.Bet(BuyIn);
            for (int i = 0; i < DrawAtStartOfRound; i++)
            {
                var card = Deck.Draw();
                if (card is not null)
                    player.AddCardToHand(card);
            }
        }
        _currentPlayer = Dealer;
        MoveToNextPlayer();
    }

    #region Choices
    /// <inheritdoc/>
    public PokerCard? Hit(BlackJackPlayer player)
    {
        var card = Deck.Draw();
        if (card is null)
            return null;

        player.AddCardToHand(card);
        Log($"{player.Name} drew a {card.Name}.");
        MoveToNextPlayer();
        return card;
    }

    /// <inheritdoc/>
    public PokerCard? DoubleDown(BlackJackPlayer player)
    {
        var card = Deck.Draw();
        if (card is null)
            return null;

        player.Bet(Math.Clamp(player.CurrentBet, 0, player.TotalPoints));
        player.AddCardToHand(card);
        player.Stand();

        Log($"{player.Name} doubled down! Their bet is now {player.CurrentBet}.");
        Log($"{player.Name} drew a {card.Name}.");
        MoveToNextPlayer();
        return card;
    }

    /// <inheritdoc/>
    public void Stand(BlackJackPlayer player)
    {
        player.Stand();
        Log($"{player.Name} is standing on {player.HandValue}.");
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
        _currentPlayer = _players[nextPlayerIndex];

        if (_currentPlayer.IsOutForRound)
        {
            MoveToNextPlayer();
            return;
        }
        if (_currentPlayer == Dealer)
        {
            PlayDealer();
            MoveToNextPlayer();
            return;
        }
        Log($"It's {_currentPlayer.Name}'s turn.");
        Prompt = $"{_currentPlayer.Name}: Choose an action!";
    }

    /// <inheritdoc/>
    public bool? PlayDealer()
    {
        if (Dealer.IsOutForRound)
            return null;

        Log($"It's the dealer's turn.");
        var averageValueOfCardPool = _dealerCanCountCards ? Deck.CardPool.Average(c => c.Value) : 7.3;
        if (Dealer.HandValue <= BlackJackScore - Math.Ceiling(averageValueOfCardPool / 2))
        {
            var newCard = Deck.Draw();
            if (newCard is null)
                return null;
            Dealer.AddCardToHand(newCard);
            Log($"The dealer drew a card.");
            return true;
        }
        else
        {
            Dealer.Stand();
            Log($"The dealer is standing on their cards.");
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
    public IEnumerable<(BlackJackPlayer Player, int Winnings)> CalculateCurrentPotentialPointGain()
    {
        (BlackJackPlayer Player, int Winnings)[] results = [];
        foreach (var player in _players.Except([Dealer]))
        {
            var winningMultiplier = -1;

            if (player.IsBusted)
            {
                winningMultiplier = 0;
            }
            else if (player.HandValue == BlackJackScore && player.Hand.Count == 2)
            {
                // blackjack
                winningMultiplier = 3;
            }
            else if ((Dealer.IsBusted && !player.IsBusted)
                || (!Dealer.IsBusted && !player.IsBusted && player.HandValue > Dealer.HandValue))
            {
                // win
                winningMultiplier = 2;
            }
            else if (!player.IsBusted && player.HandValue == Dealer.HandValue)
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

        Log($"Paying out to all players:");
        Log($"The dealer ended the round with {Dealer.HandValue}");
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            item.Player.AddPoints(item.Winnings);
            if (item.Winnings < 1)
                Log($"{item.Player.Name} had {item.Player.HandValue} and lost...");
            else if (item.Winnings == BuyIn)
                Log($"{item.Player.Name} had {item.Player.HandValue} and drew with the dealer! +{item.Winnings}");
            else if (item.Player.HandValue == BlackJackScore && item.Player.Hand.Count == 2)
                Log($"{item.Player.Name} got a blackjack! +{item.Winnings}");
            else
                Log($"{item.Player.Name} had {item.Player.HandValue} and won! +{item.Winnings}");
        }

        if (_automaticallyIncreaseStakeAfterRound > 0)
            RaiseTheStakes(_automaticallyIncreaseStakeAfterRound);
        Prompt = $"Start the next round!";
    }

    /// <inheritdoc/>
    public void RaiseTheStakes(int amount)
    {
        BuyIn += amount;
        Log($"The stakes have risen to {BuyIn}!");
    }

    /// <inheritdoc/>
    public void Log(string text)
        => _logEntries.Add(new LogEntry(text, CurrentRound));
}
