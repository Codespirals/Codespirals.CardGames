namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc cref="IFlipSevenCard"/>
public class FlipSevenCard : IFlipSevenCard
{
    /// <inheritdoc/>
    public int Value { get; init; }
    /// <inheritdoc/>
    public string Name => GetName();
    /// <inheritdoc/>
    public CardType CardType { get; init; }
    /// <inheritdoc/>
    public bool IsActionCard => CardType is CardType.Flip or CardType.Freeze or CardType.SecondChance;

    internal FlipSevenCard(CardType cardType, int value = 0)
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
            CardType.Freeze => FlipSevenConstants.FREEZENAME,
            CardType.Flip => $"{FlipSevenConstants.FLIPNAME} {Value}",
            CardType.SecondChance => FlipSevenConstants.SECONDCHANCENAME,
            _ => $"???"
        };
}
