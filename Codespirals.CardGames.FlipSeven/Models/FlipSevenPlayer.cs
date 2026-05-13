using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenPlayer : IFlipSevenPlayer<FlipSevenDeck, FlipSevenCard>
{
    private readonly FlipSevenGame _game;
    private readonly List<FlipSevenCard> _hand = [];

    public string Name { get; set; }
    public ReadOnlyCollection<FlipSevenCard> Hand => _hand.AsReadOnly();
    public int NumberCardsInHand => _hand.Count(c => c.CardType == CardType.Number);
    public int HandPoints => CalculateHandValue();
    public int TotalPoints { get; private set; } = 0;
    public bool IsOutForRound => State != PlayerStates.Playing;
    public PlayerStates State { get; private set; }
    private FlipSevenPlayer(FlipSevenGame game, string name)
    {
        _game = game;
        Name = name;
    }
    private FlipSevenPlayer(FlipSevenGame game, int number) : this(game, $"Player {number}")
    {

    }
    public static FlipSevenPlayer GeneratePlayer(FlipSevenGame game, string name)
        => new FlipSevenPlayer(game, name);
    public static FlipSevenPlayer GeneratePlayer(FlipSevenGame game, int number)
        => new FlipSevenPlayer(game, number);

    public void AddCardToHand(FlipSevenCard card)
        => _hand.Add(card);

    public void Discard(FlipSevenCard card)
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

    public int Bank()
    {
        State = PlayerStates.Banked;
        return HandPoints;
    }

    public int Freeze()
    {
        State = PlayerStates.Frozen;
        return HandPoints;
    }

    public void DeactivateForRound()
        => State = PlayerStates.Busted;

    public void Reactivate()
    {
        DiscardAll();
        State = PlayerStates.Playing;
    }

    public void AddWinnings(int points)
        => TotalPoints += points;

    private int CalculateHandValue()
    {
        var points = NumberCardsInHand == _game.NumbersToFlip ? _game.FlipNumberBonus : 0;
        foreach (var card in _hand.OrderBy(c => c.CardType))
        {
            switch (card.CardType)
            {
                case CardType.Number:
                    points += card.Value;
                    break;
                case CardType.Multiplier:
                    points *= 2;
                    break;
                case CardType.BonusAdd:
                    points += card.Value;
                    break;
                default:
                    break;
            }
        }
        return points;
    }
    public override string ToString()
        => $"{Name} {(Hand.Count != 0 ? "Hand: " : "")}{string.Join('|', Hand.OrderBy(c => c.CardType).Select(c => c.Name))} Hand total: {HandPoints} Banked:{TotalPoints}";
}
