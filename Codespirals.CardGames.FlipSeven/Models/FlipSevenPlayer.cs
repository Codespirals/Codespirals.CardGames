using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc />
public class FlipSevenPlayer : Player<FlipSevenCard>, IFlipSevenPlayer<FlipSevenCard>
{
    private readonly List<FlipSevenCard> _hand = [];

    /// <inheritdoc />
    public int NumberCardsInHand => _hand.Count(c => c.CardType == CardType.Number);
    /// <inheritdoc />
    public int TotalPoints { get; private set; } = 0;
    /// <inheritdoc />
    public bool IsBusted => State == PlayerState.Busted;
    /// <inheritdoc />
    public bool IsOutForRound => State != PlayerState.Playing;
    /// <inheritdoc />
    public PlayerState State { get; private set; }
    internal FlipSevenPlayer(string name) : base(name)
    {

    }

    /// <inheritdoc />
    public void Bank()
        => State = PlayerState.Banked;

    /// <inheritdoc />
    public void Freeze()
        => State = PlayerState.Frozen;

    /// <inheritdoc />
    public void Bust()
        => State = PlayerState.Busted;

    /// <inheritdoc />
    public void DeactivateForRound()
        => State = PlayerState.Busted;

    /// <inheritdoc />
    public void Reactivate()
    {
        DiscardAll();
        State = PlayerState.Playing;
    }

    /// <inheritdoc />
    public void AddPoints(int points)
        => TotalPoints += points;

    /// <inheritdoc />
    public override string ToString()
        => $"{Name} {(Hand.Count != 0 ? "Hand: " : "")}{string.Join('|', Hand)} | Banked: {TotalPoints}";
}
