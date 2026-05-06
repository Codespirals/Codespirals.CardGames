using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;
public class BlackJackGame : IBlackJackGame<BlackJackGame, BlackJackPlayer, Deck, Card>
{
    private readonly List<BlackJackPlayer> _players = [];
    private BlackJackPlayer _currentPlayer;
    private int _currentRound = 0;
    private int _automaticallyIncreaseStakeAfterRound = 0;
    public Deck Deck { get; }
    public BlackJackPlayer Dealer { get; }
    public int WinningScore { get; private set; } = 21;
    public int BuyIn { get; private set; }
    public int DrawAtStartOfRound { get; private set; } = 2;
    public ReadOnlyCollection<BlackJackPlayer> Players => _players.ToList().AsReadOnly();
    public BlackJackPlayer CurrentPlayer => _currentPlayer;
    public int CurrentRound => _currentRound;
    public bool RoundActive => !_players.All(p => p.IsOutForRound);
    public bool GameOver => Players.Except([Dealer]).All(p => p.TappedOut);

    public BlackJackGame(int players, int minBet = 1, int winningScore = 21, int startingCash = 100, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, Deck? deck = null)
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
        WinningScore = winningScore;
        DrawAtStartOfRound = drawAtStartOfRound;
        Deck = deck ?? PokerDeckBuilder.BasicBlackJackDeck();
        Deck.Shuffle();
    }

    public static BlackJackGame SetUp(int players) => new(players, 1, 21, 100, 1, 2);
    public static BlackJackGame SetUp(int players, int minBet, int winningScore, int startingCash, int automaticallyIncreaseStakeAfterRound, int drawAtStartOfRound) => new(players, minBet, winningScore, startingCash, automaticallyIncreaseStakeAfterRound, drawAtStartOfRound);
    public static BlackJackGame SetUp(int players, int minBet, int winningScore, int startingCash, int automaticallyIncreaseStakeAfterRound, int drawAtStartOfRound, Deck? deck)
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
    public Card Hit(BlackJackPlayer player)
    {
        var card = Deck.Draw();
        player.AddCardToHand(card);
        return card;
    }

    public Card DoubleDown(BlackJackPlayer player)
    {
        var card = Deck.Draw();
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

    public Card? PlayDealer()
    {
        if (Dealer.IsOutForRound)
            return null;

        var averageValueOfCardPool = Deck.CardPool.Average(c => c.Value);
        if (Dealer.HandValue <= WinningScore - Math.Ceiling(averageValueOfCardPool / 2))
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

        if (_automaticallyIncreaseStakeAfterRound > 0)
            RaiseTheStakes(_automaticallyIncreaseStakeAfterRound);
    }

    public (BlackJackPlayer Player, int WinningMultiplier)[] CalculateWinningsOfRound()
    {
        (BlackJackPlayer Player, int WinningMultiplier)[] results = [];
        foreach (var player in _players.Except([Dealer]))
        {
            var winningMultiplier = 0;

            if (player.IsBusted)
            {
                results = results.Append((player, winningMultiplier)).ToArray();
            }
            else if (player.HandValue == WinningScore && player.Hand.Count == 2)
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
            results = results.Append((player, winningMultiplier)).ToArray();
        }

        return results;
    }

    public void PayOut()
    {
        foreach (var item in CalculateWinningsOfRound())
        {
            item.Player.AddWinnings(item.WinningMultiplier);
        }
    }

    public void RaiseTheStakes(int amount)
    {
        BuyIn += amount;
    }
}
