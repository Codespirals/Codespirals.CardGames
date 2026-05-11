using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public abstract class Deck<TCard> : IDeck<TCard>
    where TCard : ICard
{
    internal List<TCard> _startingCards = [];
    internal List<TCard> _cardPool = [];
    internal List<TCard> _discardPile = [];

    public ReadOnlyCollection<TCard> StartingCards => _startingCards.AsReadOnly();
    public ReadOnlyCollection<TCard> CardPool => _cardPool.AsReadOnly();
    public ReadOnlyCollection<TCard> DiscardPile => _discardPile.AsReadOnly();
    public bool RefreshOnEmpty { get; set; } = true;
    public int Reshuffles { get; private set; }

    public void AddStartingCard(TCard card) => _startingCards.Add(card);

    public TCard? Draw()
    {
        if (CardPool.Count is 0)
        {
            if (RefreshOnEmpty)
            {
                ReturnDiscardPileToCardPool();
                _cardPool.Shuffle();
            }
            else
                return default;
        }
        var card = _cardPool.First();
        _cardPool.RemoveAt(0);
        return card;
    }
    public IEnumerable<TCard> Draw(int numberOfCards)
    {
        if (_cardPool.Count < numberOfCards)
        {
            if (RefreshOnEmpty)
            {
                ReturnDiscardPileToCardPool();
                _cardPool.Shuffle();
            }
            else
                numberOfCards = Math.Clamp(numberOfCards, 0, _cardPool.Count);
        }
        var cards = _cardPool.Take(numberOfCards);
        _cardPool.RemoveRange(0, numberOfCards);
        return cards;
    }
    public IEnumerable<TCard> Peek(int numberOfCards) => CardPool.Take(numberOfCards);
    public void PutOnDiscardPile(TCard card) => _discardPile.Add(card);
    public void PutOnDiscardPile(TCard[] cards) => _discardPile.AddRange(cards);
    public void ReturnDiscardPileToCardPool()
    {
        _cardPool.AddRange(_discardPile);
        _discardPile.Clear();
        Reshuffles++;
    }
    public void Shuffle() => _cardPool = [.. _cardPool.Shuffle()];
    public void Reset()
    {
        _cardPool = _startingCards;
        _discardPile = [];
        Reshuffles = 0;
        Shuffle();
    }
}
