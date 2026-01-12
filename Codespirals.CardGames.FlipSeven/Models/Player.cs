using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Player : IPlayerInGameWithOpenHand<Deck, Card>
{
    private readonly List<Card> _hand = [];
    private readonly int _numbersToFlip;

    public Deck Deck { get; }
    public int Id { get; }
    public string Name { get; set; }
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    public int HandCount => _hand.Count(c => c.CardType == CardType.Number);
    public int Points { get; private set; } = 0;
    public bool IsOutForRound => State != PlayerStates.Playing;
    public PlayerStates State { get; internal set; }
    public Player(int id, Deck deck, int numbersToFlip = 7)
    {
        Id = id;
        Deck = deck;
        Name = $"Player {Id + 1}";
        _numbersToFlip = numbersToFlip;
    }
    public Card Draw()
    {
        var card = Deck.Draw();
        if (card.CardType is CardType.Freeze or CardType.FlipThree)
            return card;
        if (card.CardType is CardType.Number)
        {
            if (!_hand.Any(c => c.Value == card.Value))
            {
                _hand.Add(card);
                return card;
            }
            var hasSecondChance = _hand.FirstOrDefault(c => c.CardType == CardType.SecondChance);
            if (hasSecondChance is not null)
            {
                Deck.PutOnDiscardPile(card);
                Discard(hasSecondChance);
                return card;
            }
            State = PlayerStates.Busted;
            DiscardAll();
        }
        _hand.Add(card);
        return card;
    }

    public void Discard(Card card)
    {
        Deck.PutOnDiscardPile(card);
        _hand.Remove(card);
    }
    public void DiscardAll()
    {
        foreach (var card in _hand)
            Deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    public void BankPoints()
    {
        var roundPoints = HandCount >= _numbersToFlip ? 15 : 0;
        foreach (var card in _hand.OrderBy(c => c.CardType))
        {
            switch (card.CardType)
            {
                case CardType.Number:
                    roundPoints += card.Value;
                    break;
                case CardType.TimesTwo:
                    roundPoints *= 2;
                    break;
                case CardType.BonusAdd:
                    roundPoints += card.Value;
                    break;
                default:
                    break;
            }
        }
        DiscardAll();
        Points += roundPoints;
        State = PlayerStates.Banked;
    }
    public void Freeze()
    {
        BankPoints();
        State = PlayerStates.Frozen;
    }
}
