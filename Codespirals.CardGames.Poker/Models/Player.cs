using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker;
public class Player<TGame> : IPokerPlayer<Deck, Card>
    where TGame : IGame<TGame, Deck, Card>
{
    private readonly TGame _game;
    private readonly List<Card> _hand = [];
    private bool _isOutForRound;

    public string Name { get; set; }
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    public int HandCount => _hand.Count;
    public int HandValue => CalculateHandValue();
    public int Points { get; private set; }
    public int CurrentBet { get; private set; }
    public bool IsOutForRound => _isOutForRound || TappedOut;
    public bool TappedOut => Points == 0 && CurrentBet == 0;

    public Player(TGame game, int id, int startingPoints = 100)
    {
        _game = game;
        Name = $"Player {id + 1}";
        Points = startingPoints;
    }
    public Player(TGame game, string name, int startingPoints = 100)
    {
        _game = game;
        Name = name;
        Points = startingPoints;
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

    public void AddCardToHand(Card card) => _hand.Add(card);

    public void Bet(int amount)
    {
        CurrentBet = Math.Clamp(amount, 0, Points);
        Points -= CurrentBet;
    }

    public void Stand()
        => _isOutForRound = true;

    public void AddWinnings(int amount)
    {
        DiscardAll();
        Points += amount;
        CurrentBet = 0;
    }

    public void Bust()
    {
        DiscardAll();
        CurrentBet = 0;
        _isOutForRound = true;
    }
    public void Reactivate()
    {
        DiscardAll();
        _isOutForRound = false;
    }

    internal int CalculateHandValue()
    {
        var value = _hand.Sum(c => c.Value);
        foreach (var ace in _hand.Where(c => c.Value == 11))
        {
            if (value <= 21)
                break;
            value -= 10;
        }
        if (value > 21)
        {
            Bust();
        }
        return value;
    }
    public override string ToString()
        => $"{Name} {(Hand.Count != 0 ? "Hand: " : "")}{string.Join('|', Hand.OrderBy(c => c.Suit).Select(c => c.Name))} Hand total: {HandValue} Cash:{Points} Current Bet:{CurrentBet}";

}
