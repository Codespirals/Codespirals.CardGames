using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Deck : IDeck<Card>
{
    private List<Card> _startingCards = [];
    private List<Card> _cardPool = [];
    private List<Card> _discardPile = [];

    public ReadOnlyCollection<Card> StartingCards => _startingCards.AsReadOnly();
    public ReadOnlyCollection<Card> CardPool => _cardPool.AsReadOnly();
    public ReadOnlyCollection<Card> DiscardPile => _discardPile.AsReadOnly();

    public Deck(int numberCards = 12, int freezes = 4, int flipThrees = 4, int secondChances = 4, int timesTwos = 1, int bonusCards = 5)
    {
        for (var v = numberCards; v < 0; v--)
        {
            for (var i = 0; i < v; i++)
            {
                _startingCards.Add(new Card(CardType.Number, v));
            }
        }
        for (var i = 0; i < bonusCards; i++)
        {
            _startingCards.Add(new Card(CardType.BonusAdd, i * 2));
        }
        for (var i = 0; i < timesTwos; i++)
        {
            _startingCards.Add(new Card(CardType.TimesTwo, 1));
        }
        for (var i = 0; i < flipThrees; i++)
        {
            _startingCards.Add(new Card(CardType.FlipThree, 2));
        }
        for (var i = 0; i < freezes; i++)
        {
            _startingCards.Add(new Card(CardType.Freeze, 3));
        }
        for (var i = 0; i < secondChances; i++)
        {
            _startingCards.Add(new Card(CardType.SecondChance, 4));
        }
        _cardPool = _startingCards;
    }

    public Card? Draw()
    {
        if (CardPool.Count is 0)
            return default;
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
    public void Shuffle() => _cardPool.Shuffle();
    public void Discard(Card card) => _discardPile.Add(card);
    public void ReturnDiscardPile()
    {
        _cardPool.AddRange(_discardPile);
        _discardPile = [];
    }
    public void Order() => _cardPool = [.. _cardPool.OrderBy(c => c.CardType).ThenBy(c => c.Value)];
    public void Reset()
    {
        _cardPool = _startingCards;
        _discardPile = [];
        Order();
    }
}
