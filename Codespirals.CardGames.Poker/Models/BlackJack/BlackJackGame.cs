using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc cref="IBlackJackGame{TSelf, TPlayer, TDeck, TCard}"/>
public class BlackJackGame : IBlackJackGame<BlackJackGame, BlackJackPlayer, PokerDeck, PokerCard>
{
    private readonly List<BlackJackPlayer> _players = [];
    private BlackJackPlayer _currentPlayer;
    private int _currentRound = 0;
    private bool _dealerCanCountCards;
    private List<LogEntry> _logEntries = [];

    /// <inheritdoc/>
    public PokerDeck Deck { get; } = PokerDeckBuilder.CreateBlackJackDeck();
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
    public int BuyIn { get; private set; } = 1;
    /// <inheritdoc/>
    public int DrawAtStartOfRound { get; private set; } = 2;
    /// <inheritdoc/>
    public int IncreaseStakeAfterRound { get; private set; } = 1;
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
        Dealer = PokerPlayerGenerator.GenerateBlackJackDealer(winningScore);
        _dealerCanCountCards = dealerCanCountCards;
        _players.Add(Dealer);
        _currentPlayer = Dealer;

        for (var i = 0; i < players; i++)
        {
            _players.Add(PokerPlayerGenerator.GenerateBlackJackPlayer(i, startingCash, winningScore));
        }
        IncreaseStakeAfterRound = automaticallyIncreaseStakeAfterRound;
        BuyIn = minBet;
        BlackJackScore = winningScore;
        DrawAtStartOfRound = drawAtStartOfRound;
        Deck = deck ?? PokerDeckBuilder.CreateBlackJackDeck();
        Deck.Shuffle();
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
        _currentRound++;
        Log($"Starting round {_currentRound}");
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
        _currentPlayer = Dealer;
        MoveToNextPlayer();
    }

    #region Choices
    /// <inheritdoc/>
    public PokerCard? Hit()
    {
        var card = Deck.Draw();
        if (card is null)
            return null;

        _currentPlayer.AddCardToHand(card);
        Log($"{_currentPlayer.Name} drew a {card.Name}.", GetPlayerId(_currentPlayer));
        MoveToNextPlayer();
        return card;
    }

    /// <inheritdoc/>
    public PokerCard? DoubleDown()
    {
        var card = Deck.Draw();
        if (card is null)
            return null;

        _currentPlayer.Bet(Math.Clamp(_currentPlayer.CurrentBet, 0, _currentPlayer.TotalPoints));
        _currentPlayer.AddCardToHand(card);
        _currentPlayer.Stand();

        Log($"{_currentPlayer.Name} doubled down! Their bet is now {_currentPlayer.CurrentBet}.", GetPlayerId(_currentPlayer));
        Log($"{_currentPlayer.Name} drew a {card.Name}.", GetPlayerId(_currentPlayer));
        MoveToNextPlayer();
        return card;
    }

    /// <inheritdoc/>
    public void Stand()
    {
        _currentPlayer.Stand();
        Log($"{_currentPlayer.Name} is standing on {_currentPlayer.HandValue}.", GetPlayerId(_currentPlayer));
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
        Log($"It's {_currentPlayer.Name}'s turn.", GetPlayerId(_currentPlayer));
        Prompt = $"{_currentPlayer.Name}: Choose an action!";
    }

    /// <inheritdoc/>
    public bool? PlayDealer()
    {
        if (Dealer.IsOutForRound)
            return null;

        Log($"It's the dealer's turn.", GetPlayerId(Dealer));
        var averageValueOfCardPool = _dealerCanCountCards ? Deck.CardPool.Average(c => c.Value) : 7.3;
        if (Dealer.HandValue <= BlackJackScore - Math.Ceiling(averageValueOfCardPool / 2))
        {
            var newCard = Deck.Draw();
            if (newCard is null)
                return null;
            Dealer.AddCardToHand(newCard);
            Log($"The dealer drew a card.", GetPlayerId(Dealer));
            return true;
        }
        else
        {
            Dealer.Stand();
            Log($"The dealer is standing on their cards.", GetPlayerId(Dealer));
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

        Log(new string('=', 20));

        Log($"Paying out to all players:");
        Log($"The dealer ended the round with {Dealer.HandValue}", GetPlayerId(Dealer));
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            item.Player.AddPoints(item.Winnings);
            if (item.Winnings < 1)
                Log($"{item.Player.Name} had {item.Player.HandValue} and lost...", GetPlayerId(item.Player));
            else if (item.Winnings == BuyIn)
                Log($"{item.Player.Name} had {item.Player.HandValue} and drew with the dealer! +{item.Winnings}", GetPlayerId(item.Player));
            else if (item.Player.HandValue == BlackJackScore && item.Player.Hand.Count == 2)
                Log($"{item.Player.Name} got a blackjack! +{item.Winnings}", GetPlayerId(item.Player));
            else
                Log($"{item.Player.Name} had {item.Player.HandValue} and won! +{item.Winnings}", GetPlayerId(item.Player));
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
