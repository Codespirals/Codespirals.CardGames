using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker;
public class Deck : IPokerDeck<Card>
{
    private List<Card> _startingCards = [];
    private List<Card> _cardPool = [];
    private List<Card> _discardPile = [];

    public ReadOnlyCollection<Card> StartingCards => _startingCards.AsReadOnly();
    public ReadOnlyCollection<Card> CardPool => _cardPool.AsReadOnly();
    public ReadOnlyCollection<Card> DiscardPile => _discardPile.AsReadOnly();

    internal Deck()
    {
        
    }
    internal void AddStartingCard(Card card) => _startingCards.Add(card); 
    internal void OrderStartingCards() => _startingCards = [.. _startingCards.OrderBy(c => c.Suit).ThenBy(c => c.Value)];

    public Card Draw()
    {
        if (CardPool.Count is 0)
            return CardHelper.NoCard();
        var card = CardPool.First();
        _discardPile.Add(card);
        return card;
    }
    public IEnumerable<Card> Draw(int numberOfCards)
    {
        var cards = _cardPool.Take(numberOfCards);
        _cardPool.RemoveRange(0, numberOfCards);
        return cards;
    }
    public IEnumerable<Card> Peek(int numberOfCards) => CardPool.Take(numberOfCards);
    public void PutOnDiscardPile(Card card) => _discardPile.Add(card);
    public void ShuffleDiscardPileIntoDeck()
    {
        _cardPool.AddRange(_discardPile);
        _discardPile.Clear();
    }
    public void Shuffle() => _cardPool = [.. _cardPool.Shuffle()];
    public void Order() => _cardPool = [.. _cardPool.OrderBy(c => c.Suit).ThenBy(c => c.Value)];
    public void Reset()
    {
        _cardPool = _startingCards;
        _discardPile = [];
        Order();
    }
}
