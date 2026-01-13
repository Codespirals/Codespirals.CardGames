using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Deck : IFlipSevenDeck<Card>
{
    private List<Card> _startingCards = [];
    private List<Card> _cardPool = [];
    private List<Card> _discardPile = [];

    public ReadOnlyCollection<Card> StartingCards => _startingCards.AsReadOnly();
    public ReadOnlyCollection<Card> CardPool => _cardPool.AsReadOnly();
    public ReadOnlyCollection<Card> DiscardPile => _discardPile.AsReadOnly();
    public int Reshuffles { get; private set; }
    internal Deck()
    {
        
    }

    internal void AddStartingCard(Card card) => _startingCards.Add(card);
    internal void OrderStartingCards() => _startingCards = _startingCards.OrderBy(c => c.CardType).ThenBy(c => c.Value).ToList();
    public Card Draw()
    {
        if (CardPool.Count is 0)
            ShuffleDiscardPileIntoDeck();
        var card = _cardPool.First();
        _cardPool.Remove(card);
        return card;
    }

    public IEnumerable<Card> Draw(int numberOfCards)
    {
        var clamped = Math.Clamp(numberOfCards, 0, _cardPool.Count);
        var cards = _cardPool.Take(clamped);
        _cardPool.RemoveRange(0, clamped);
        return cards;
    }

    public IEnumerable<Card> Peek(int numberOfCards) => _cardPool.Take(Math.Clamp(numberOfCards, 0, _cardPool.Count));
    public void PutOnDiscardPile(Card card) => _discardPile.Add(card);
    public void ShuffleDiscardPileIntoDeck()
    {
        _cardPool.AddRange(_discardPile);
        _discardPile = [];
        Reshuffles++;
    }
    public void Shuffle() => _cardPool = [.. _cardPool.Shuffle()];
    public void Order() => _cardPool = [.. _cardPool.OrderBy(c => c.CardType).ThenBy(c => c.Value)];
    public void Reset()
    {
        _cardPool = _startingCards;
        _discardPile = [];
        Reshuffles = 0;
        Order();
    }
}
