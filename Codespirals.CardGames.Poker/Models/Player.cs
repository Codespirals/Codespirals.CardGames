using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker;
public class Player<TGame> : IPokerPlayer<Deck, Card>
    where TGame : IGame<TGame, Player<TGame>, Deck, Card>
{
    private readonly TGame _game;
    private readonly List<Card> _hand = [];
    private readonly int _playerNumber = 0;

    public string Name {  get; set; }
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    public int HandCount => _hand.Count;
    public int Points { get; internal set; }
    public int CurrentBet { get; internal set; }

    public Player(TGame game, int id)
    {
        _game = game;
        _playerNumber = id;
        Name = $"Player {_playerNumber + 1}";
    }

    public void Discard(Card card)
    {
        _hand.Remove(card);
        _game.Deck.PutOnDiscardPile(card);
    }

    public void DiscardAll()
    {
        foreach (var card in _hand)
            _game.Deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    public void Draw(Card card) => _hand.Add(card);
    public void Bet(int amount)
    {

    }
}
