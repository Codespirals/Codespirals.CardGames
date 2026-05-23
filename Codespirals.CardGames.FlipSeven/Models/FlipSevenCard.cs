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

    private FlipSevenCard(CardType cardType, int value = 0)
    {
        CardType = cardType;
        Value = value;
    }
    public static FlipSevenCard GenerateNumberCard(int value)
        => new FlipSevenCard(CardType.Number, value);
    public static FlipSevenCard GenerateMultiplierCard(int value)
        => new FlipSevenCard(CardType.Multiplier, value);
    public static FlipSevenCard GenerateBonusAddCard(int value)
        => new FlipSevenCard(CardType.BonusAdd, value);
    public static FlipSevenCard GenerateSecondChanceCard()
        => new FlipSevenCard(CardType.SecondChance);
    public static FlipSevenCard GenerateFlipCard(int value)
        => new FlipSevenCard(CardType.Flip, value);
    public static FlipSevenCard GenerateFreezeCard()
        => new FlipSevenCard(CardType.Freeze);

    /// <inheritdoc/>
    public override string ToString()
        => Name;

    private string GetName()
        => CardType switch
        {
            CardType.Freeze => FlipSevenConstants.FREEZENAME,
            CardType.Flip => $"{FlipSevenConstants.FLIPNAME} {Value}",
            CardType.SecondChance => FlipSevenConstants.SECONDCHANCENAME,
            CardType.BonusAdd => $"+{Value}",
            CardType.Multiplier => $"x{Value}",
            CardType.Number => $"{Value}",
            _ => $"???"
        };
}
