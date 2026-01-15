using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker;
public class BlackJack : IBlackJackGame<BlackJack, Player<BlackJack>, Deck, Card>
{
    private readonly List<Player<BlackJack>> _players = [];
    private Player<BlackJack> _currentPlayer;
    private readonly int _currentRound = 0;
    public Deck Deck { get; } = PokerDeckBuilder.BasicBlackJackDeck();
    public Player<BlackJack> Dealer { get; }
    public int WinningScore { get; private set; } = 21;
    public int MinBet { get; private set; }
    public ReadOnlyCollection<Player<BlackJack>> Players => new List<Player<BlackJack>>([Dealer]).Concat(_players).ToList().AsReadOnly();
    public int CurrentRound => _currentRound + 1;
    public bool RoundActive { get; private set; }
    public bool GameOver { get; private set; }

    public BlackJack(int players, int minBet, int winningScore = 21)
    {
        Dealer = new Player<BlackJack>(this, "Dealer", 1000000);
        for (var i = 0; i < players; i++)
        {
            _players.Add(new Player<BlackJack>(this, i));
        }
        _currentPlayer = _players.First();
        MinBet = minBet;
        WinningScore = winningScore;
    }

    public static BlackJack SetUp(int players) => new(players, 1, 21);
    public static BlackJack SetUp(int players, int minBet, int winningScore) => new(players, minBet, winningScore);

    public Player<BlackJack> GetCurrentPlayer()
        => _currentPlayer;
    public void StartRound()
    {
        RoundActive = true;
        foreach (var player in _players)
        {
            player.Bet(MinBet);
            player.AddCardToHand(Deck.Draw());
            player.AddCardToHand(Deck.Draw());
        }
        _currentPlayer = _players[CurrentRound % _players.Count];
    }

    public void MoveToNextPlayer()
    {
        if (Players.All(p => p.TappedOut))
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

    public Card Hit(Player<BlackJack> player)
    {
        var card = Deck.Draw();
        player.AddCardToHand(card);
        return card;
    }
    public Card DoubleDown(Player<BlackJack> player)
    {
        player.Bet(player.CurrentBet);
        return Hit(player);
    }
    public void Stand(Player<BlackJack> player) => player.Stand();

    public void EndRound()
    {
        RoundActive = false;
        foreach (var player in _players)
        {
            player.Reactivate();
        }
    }
}
