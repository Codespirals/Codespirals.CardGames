namespace Codespirals.CardGames.FlipNumber;

/// <inheritdoc cref="IFlipNumberCard"/>
public class FlipNumberCard : IFlipNumberCard
{
    /// <inheritdoc/>
    public int Value { get; init; }
    /// <inheritdoc/>
    public string Name => GetName();
    /// <inheritdoc/>
    public CardType CardType { get; init; }
    /// <inheritdoc/>
    public bool IsActionCard => CardType is CardType.Flip or CardType.ForceBank or CardType.Protection;

    internal FlipNumberCard(CardType cardType, int value = 0)
    {
        CardType = cardType;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
        => Name;

    private string GetName()
        => CardType switch
        {
            CardType.Number => $"{Value}",
            CardType.Multiplier => $"x{Value}",
            CardType.BonusAdd => Value > 0 ? $"+{Value}" : $"{Value}",
            CardType.ForceBank => FlipNumberConstants.FORCEBANKNAME,
            CardType.Flip => $"{FlipNumberConstants.FLIPNAME} {Value}",
            CardType.Protection => FlipNumberConstants.PROTECTIONNAME,
            _ => $"???"
        };
}
