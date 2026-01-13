using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Player : IFlipSevenPlayer<Deck, Card>
{
    private readonly Game _game;
    private readonly List<Card> _hand = [];
    private readonly int _playerNumber = 0;

    public string Name { get; set; }
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    public int HandCount => _hand.Count(c => c.CardType == CardType.Number);
    public int Points { get; private set; } = 0;
    public bool IsOutForRound => State != PlayerStates.Playing;
    public PlayerStates State { get; private set; }
    public Player(Game game, int id)
    {
        _game = game;
        _playerNumber = id;
        Name = $"Player {_playerNumber + 1}";
    }

    public void Draw(Card card)
    {
        if (card.CardType is CardType.Flip or CardType.Freeze)
        {
            Discard(card);
            return;
        }
        if (card.CardType == CardType.Number && Hand.Any(c => c.CardType == CardType.Number && c.Value == card.Value))
        {
            Discard(card);
            Bust();
            return;
        }
        _hand.Add(card);
    }

    public void Discard(Card card)
    {
        _game.Deck.PutOnDiscardPile(card);
        _hand.Remove(card);
    }

    public void DiscardAll()
    {
        foreach (var card in _hand)
            _game.Deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    public void BankPoints()
    {
        var roundPoints = HandCount >= _game.NumbersToFlip ? 15 : 0;
        foreach (var card in _hand.OrderBy(c => c.CardType))
        {
            switch (card.CardType)
            {
                case CardType.Number:
                    roundPoints += card.Value;
                    break;
                case CardType.Multiplier:
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
    public void Bust()
    {
        var secondChance = Hand.FirstOrDefault(c => c.CardType == CardType.SecondChance);
        if (secondChance is not null)
        {
            Discard(secondChance);
            return;
        }
        DiscardAll();
        State = PlayerStates.Busted;
    }
    public void Reactivate()
    {
        DiscardAll();
        State = PlayerStates.Playing;
    }

    public override string ToString()
        => $"{Name} {(Hand.Count != 0 ? "Hand: " : "")}{string.Join('|', Hand.OrderBy(c => c.CardType).Select(c => c.Value))} Total Points: ({Points})";
}
