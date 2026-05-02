using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;
public class BlackJackGame : IBlackJackGame<BlackJackGame, BlackJackPlayer, Deck, Card>
{
    private readonly List<BlackJackPlayer> _players = [];
    private BlackJackPlayer _currentPlayer;
    private int _currentRound = 0;
    public Deck Deck { get; }
    public BlackJackPlayer Dealer { get; }
    public int WinningScore { get; private set; } = 21;
    public int BuyIn { get; private set; }
    public ReadOnlyCollection<BlackJackPlayer> Players => _players.ToList().AsReadOnly();
    public int CurrentRound => _currentRound + 1;
    public bool RoundActive => _players.Any(p => !p.IsOutForRound);
    public bool GameOver { get; private set; }

    public BlackJackGame(int players, int minBet, int winningScore = 21, int startingCash = 100)
    {
        Dealer = new BlackJackPlayer(this, "Dealer", 0);
        _players.Add(Dealer);
        _currentPlayer = Dealer;
        for (var i = 0; i < players; i++)
        {
            _players.Add(new BlackJackPlayer(this, i, startingCash));
        }
        BuyIn = minBet;
        WinningScore = winningScore;
        Deck = PokerDeckBuilder.BasicBlackJackDeck();
        Deck.Shuffle();
    }

    public static BlackJackGame SetUp(int players) => new(players, 1, 21, 100);
    public static BlackJackGame SetUp(int players, int minBet, int winningScore, int startingCash) => new(players, minBet, winningScore, startingCash);

    public BlackJackPlayer GetCurrentPlayer()
        => _currentPlayer;
    public void StartRound()
    {
        Dealer.Reactivate();
        Dealer.DiscardAll();
        Dealer.AddCardToHand(Deck.Draw());
        Dealer.AddCardToHand(Deck.Draw());
        foreach (var player in _players.Except([Dealer]))
        {
            if (player.TappedOut)
                continue;
            player.Reactivate();
            player.Bet(BuyIn);
            player.AddCardToHand(Deck.Draw());
            player.AddCardToHand(Deck.Draw());
        }
        _currentPlayer = Dealer;
    }

    public void MoveToNextPlayer()
    {
        if (Players.Except([Dealer]).All(p => p.TappedOut))
        {
            GameOver = true;
            return;
        }
        if (Players.All(p => p.IsOutForRound))
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

    public void EndRound()
    {
        _currentRound++;
    }
    public void RaiseTheStakes(int amount)
    {
        BuyIn += amount;
    }
}
