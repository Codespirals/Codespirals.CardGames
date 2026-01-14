using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;
public class BlackJack : IBlackJackGame<BlackJack, Player<BlackJack>, Deck, Card>
{
    private readonly List<Player<BlackJack>> _players = [];
    private Player<BlackJack> _currentPlayer;
    private int _currentRound = 0;
    public Deck Deck { get; } = PokerDeckBuilder.BasicBlackJackDeck();
    public ReadOnlyCollection<Player<BlackJack>> Players => _players.AsReadOnly();
    public int CurrentRound => _currentRound + 1;

    public BlackJack(int players)
    {
        for (var i = 0; i < players; i++)
        {
            _players.Add(new Player<BlackJack>(this, i));
        }
        _currentPlayer = _players.First();
    }

    public static BlackJack SetUp(int players) => new(players);

    public Card Hit(Player<BlackJack> player)
    {

    }
    public Card DoubleDown(Player<BlackJack> player)
    {

    }
    public void Stand(Player<BlackJack> player)
    {

    }

}
