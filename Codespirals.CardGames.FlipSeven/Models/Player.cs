using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Player : IFlipSevenPlayer<Deck, Card>
{
    private readonly List<Card> _hand = [];
    private readonly int _playerNumber = 0;

    public string Name { get; set; }
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    public int HandCount => _hand.Count(c => c.CardType == CardType.Number);
    public int Points { get; private set; } = 0;
    public bool IsOutForRound => State != PlayerStates.Playing;
    public PlayerStates State { get; private set; }
    public Player(int id)
    {
        _playerNumber = id;
        Name = $"Player {_playerNumber}";
    }
    public void AddCardToHand(Card card) => _hand.Add(card);

    public void Discard(Card card, Deck deck)
    {
        deck.PutOnDiscardPile(card);
        _hand.Remove(card);
    }
    public void DiscardAll(Deck deck)
    {
        foreach (var card in _hand)
            deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    public void BankPoints(Deck deck, int numberToFlip = 7)
    {
        var roundPoints = HandCount >= numberToFlip ? 15 : 0;
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
        DiscardAll(deck);
        Points += roundPoints;
        State = PlayerStates.Banked;
    }
    public void Freeze(Deck deck)
    {
        BankPoints(deck);
        State = PlayerStates.Frozen;
    }
    public void Bust(Deck deck)
    {
        DiscardAll(deck);
        State = PlayerStates.Busted;
    }
    public void Reactivate(Deck deck)
    {
        DiscardAll(deck);
        State = PlayerStates.Playing;
    }
}
