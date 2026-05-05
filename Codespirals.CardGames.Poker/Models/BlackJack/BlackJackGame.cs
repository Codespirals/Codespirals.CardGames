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
    public int CurrentRound => _currentRound + 1;
    public bool RoundActive => _players.All(p => p.IsOutForRound);
    public bool GameOver => Players.Except([Dealer]).All(p => p.TappedOut);

    public BlackJackGame(int players, int minBet = 1, int winningScore = 21, int startingCash = 100, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2)
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
        Deck = PokerDeckBuilder.BasicBlackJackDeck();
        Deck.Shuffle();
    }

    public static BlackJackGame SetUp(int players) => new(players, 1, 21, 100, 1);
    public static BlackJackGame SetUp(int players, int minBet, int winningScore, int startingCash, int automaticallyIncreaseStakeAfterRound, int drawAtStartOfRound) => new(players, minBet, winningScore, startingCash, automaticallyIncreaseStakeAfterRound, drawAtStartOfRound);

    public BlackJackPlayer GetCurrentPlayer()
        => _currentPlayer;

    public void StartRound()
    {
        Dealer.Reactivate();
        Dealer.DiscardAll();
        for (int i = 0; i < DrawAtStartOfRound; i++)
        {
            Dealer.AddCardToHand(Deck.Draw());
        }
        foreach (var player in _players.Except([Dealer]))
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

    public void MoveToNextPlayer()
    {
        if (Players.All(p => p.IsOutForRound) || GameOver)
        {
            EndRound();
            return;
        }
        var nextPlayerIndex = (Players.IndexOf(GetCurrentPlayer()) + 1) % Players.Count;
        _currentPlayer = Players[nextPlayerIndex];
        if (_currentPlayer.IsOutForRound)
            MoveToNextPlayer();
    }

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

    private bool EvaluateHand(BlackJackPlayer player)
    {
        var busted = player.HandValue > WinningScore;
        if (busted)
            player.Bust();

        if (RoundActive)
            MoveToNextPlayer();
        else
            EndRound();
        return busted;
    }

    public void EndRound()
    {
        _currentRound++;
        if (_automaticallyIncreaseStakeAfterRound > 0)
        {
            RaiseTheStakes(_automaticallyIncreaseStakeAfterRound);
        }
    }

    /// <summary>
    /// Make the dealer play his round.
    /// </summary>
    /// <remarks>The dealer counts cards. The house has an advantage.</remarks>
    public void PlayDealer()
    {
        if (!Dealer.IsOutForRound)
            return;

        var averageValueOfCardPool = Deck.CardPool.Average(c => c.Value);
        if (Dealer.HandValue <= WinningScore - averageValueOfCardPool - 1)
        {
            Hit(Dealer);
            if (Dealer.HandValue > WinningScore)
                EndRound();
        }
        else
        {
            Stand(Dealer);
        }
    }

    public void RaiseTheStakes(int amount)
    {
        BuyIn += amount;
    }
}
