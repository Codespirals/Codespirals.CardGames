using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc />
public class FlipSevenPlayer : IFlipSevenPlayer<FlipSevenDeck, FlipSevenCard>
{
    private readonly FlipSevenGame _game;
    private readonly List<FlipSevenCard> _hand = [];

    /// <inheritdoc />
    public string Name { get; set; }
    /// <inheritdoc />
    public ReadOnlyCollection<FlipSevenCard> Hand => _hand.AsReadOnly();
    /// <inheritdoc />
    public int NumberCardsInHand => _hand.Count(c => c.CardType == CardType.Number);
    /// <inheritdoc />
    public int HandPoints => CalculateHandValue();
    /// <inheritdoc />
    public int TotalPoints { get; private set; } = 0;
    /// <inheritdoc />
    public bool IsBusted => State == PlayerStates.Busted;
    /// <inheritdoc />
    public bool IsOutForRound => State != PlayerStates.Playing;
    /// <inheritdoc />
    public PlayerStates State { get; private set; }
    private FlipSevenPlayer(FlipSevenGame game, string name)
    {
        _game = game;
        Name = name;
    }
    private FlipSevenPlayer(FlipSevenGame game, int number) : this(game, $"Player {number}")
    {

    }
    /// <inheritdoc />
    public static FlipSevenPlayer GeneratePlayer(FlipSevenGame game, string name)
        => new FlipSevenPlayer(game, name);
    /// <inheritdoc />
    public static FlipSevenPlayer GeneratePlayer(FlipSevenGame game, int number)
        => new FlipSevenPlayer(game, number);

    /// <inheritdoc />
    public void AddCardToHand(FlipSevenCard card)
        => _hand.Add(card);

    /// <inheritdoc />
    public void Discard(FlipSevenCard card)
    {
        _game.Deck.PutOnDiscardPile(card);
        _hand.Remove(card);
    }

    /// <inheritdoc />
    public void DiscardAll()
    {
        foreach (var card in _hand)
            _game.Deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    /// <inheritdoc />
    public void Bank()
        => State = PlayerStates.Banked;

    /// <inheritdoc />
    public void Freeze()
        => State = PlayerStates.Frozen;

    /// <inheritdoc />
    public void Bust()
        => DeactivateForRound();

    /// <inheritdoc />
    public void DeactivateForRound()
        => State = PlayerStates.Busted;

    /// <inheritdoc />
    public void Reactivate()
    {
        DiscardAll();
        State = PlayerStates.Playing;
    }

    /// <inheritdoc />
    public void AddPoints(int points)
        => TotalPoints += points;

    private int CalculateHandValue()
    {
        if (IsBusted)
            return 0;

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
    /// <inheritdoc />
    public override string ToString()
        => $"{Name} {(Hand.Count != 0 ? "Hand: " : "")}{string.Join('|', Hand.OrderBy(c => c.CardType).Select(c => c.Name))} Hand total: {HandPoints} Banked:{TotalPoints}";
}
