using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
/// <inheritdoc cref="IDeck{TCard}"/>
public abstract class Deck<TCard> : IDeck<TCard>
    where TCard : ICard
{
    internal List<TCard> _startingCards = [];
    internal List<TCard> _cardPool = [];
    internal List<TCard> _discardPile = [];

    /// <inheritdoc/>
    public ReadOnlyCollection<TCard> StartingCards => _startingCards.AsReadOnly();
    /// <inheritdoc/>
    public ReadOnlyCollection<TCard> CardPool => _cardPool.AsReadOnly();
    /// <inheritdoc/>
    public ReadOnlyCollection<TCard> DiscardPile => _discardPile.AsReadOnly();
    /// <inheritdoc/>
    public bool RefreshOnEmpty { get; set; } = true;
    /// <inheritdoc/>
    public int Reshuffles { get; private set; }
    /// <inheritdoc/>
    public void AddStartingCard(TCard card) => _startingCards.Add(card);
    /// <inheritdoc/>
    public TCard? Draw()
    {
        if (CardPool.Count < 1)
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
    /// <inheritdoc/>
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
    /// <inheritdoc/>
    public IEnumerable<TCard> Peek(int numberOfCards) => CardPool.Take(numberOfCards);
    /// <inheritdoc/>
    public void PutOnDiscardPile(TCard card) => _discardPile.Add(card);
    /// <inheritdoc/>
    public void PutOnDiscardPile(IEnumerable<TCard> cards) => _discardPile.AddRange(cards);
    /// <inheritdoc/>
    public void ReturnDiscardPileToCardPool()
    {
        _cardPool.AddRange(_discardPile);
        _discardPile.Clear();
        Reshuffles++;
        Shuffle();
    }
    /// <inheritdoc/>
    public void Shuffle() => _cardPool = [.. _cardPool.Shuffle()];
    /// <inheritdoc/>
    public void Reset()
    {
        _cardPool = _startingCards;
        _discardPile = [];
        Reshuffles = 0;
        Shuffle();
    }
}
