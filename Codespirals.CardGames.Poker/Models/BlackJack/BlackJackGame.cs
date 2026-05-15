using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;
public class BlackJackGame : IBlackJackGame<BlackJackGame, BlackJackPlayer, PokerDeck, PokerCard>
{
    private readonly List<BlackJackPlayer> _players = [];
    private BlackJackPlayer _currentPlayer;
    private int _currentRound = 0;
    private int _automaticallyIncreaseStakeAfterRound = 0;
    private bool _dealerCanCountCards;
    public PokerDeck Deck { get; }
    public ReadOnlyCollection<BlackJackPlayer> Players => _players.ToList().AsReadOnly();
    public BlackJackPlayer CurrentPlayer => _currentPlayer;
    public int CurrentRound => _currentRound;
    public BlackJackPlayer Dealer { get; }
    public int BlackJackScore { get; private set; } = 21;
    public int BuyIn { get; private set; }
    public int DrawAtStartOfRound { get; private set; } = 2;
    public bool RoundActive => !_players.All(p => p.IsOutForRound);
    public bool GameOver => Players.Except([Dealer]).All(p => p.TappedOut);

    public BlackJackGame(int players, int minBet = 1, int blackJackScore = 21, int startingCash = 100, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, PokerDeck? deck = null, bool dealerCanCountCards = false)
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
    }

    public static BlackJackGame SetUp(int players) => new(players, 1, 21, 100, 1, 2);
    public static BlackJackGame SetUp(int players, int minBet, int winningScore, int startingCash, int automaticallyIncreaseStakeAfterRound, int drawAtStartOfRound, PokerDeck? deck)
        => new(players, minBet, winningScore, startingCash, automaticallyIncreaseStakeAfterRound, drawAtStartOfRound, deck);

    public void StartRound()
    {
        _currentRound++;
        foreach (var player in _players)
        {
            if (player.TappedOut)
                continue;
            player.Reactivate();
            player.Bet(BuyIn);
            for (int i = 0; i < DrawAtStartOfRound; i++)
            {
                player.AddCardToHand(Deck.Draw());
            }
        }
        _currentPlayer = Dealer;
    }

    #region Choices
    public PokerCard? Hit(BlackJackPlayer player)
    {
        var card = Deck.Draw();
        if (card is not null)
            player.AddCardToHand(card);
        return card;
    }

    public PokerCard? DoubleDown(BlackJackPlayer player)
    {
        var card = Deck.Draw();
        if (card is not null)
            player.DoubleDown(card);
        return card;
    }

    public void Stand(BlackJackPlayer player) => player.Stand();
    #endregion

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
    }

    public PokerCard? PlayDealer()
    {
        if (Dealer.IsOutForRound)
            return null;

        var averageValueOfCardPool = _dealerCanCountCards ? Deck.CardPool.Average(c => c.Value) : 7.3;
        if (Dealer.HandValue <= BlackJackScore - Math.Ceiling(averageValueOfCardPool / 2))
        {
            var newCard = Hit(Dealer);
            return newCard;
        }
        else
        {
            Dealer.Stand();
            return null;
        }
    }

    public void EndRound()
    {
        foreach (var player in _players)
        {
            player.DeactivateForRound();
        }
    }

    public (BlackJackPlayer Player, int Winnings)[] CalculateCurrentPotentialPointGain()
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

    public void PayOut()
    {
        if (Players.Any(p => !p.IsOutForRound))
        {
            return;
        }
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            item.Player.AddWinnings(item.Winnings);
        }

        if (_automaticallyIncreaseStakeAfterRound > 0)
            RaiseTheStakes(_automaticallyIncreaseStakeAfterRound);
    }

    public void RaiseTheStakes(int amount)
    {
        BuyIn += amount;
    }
}
