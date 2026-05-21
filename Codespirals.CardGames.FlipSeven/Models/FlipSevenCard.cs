namespace Codespirals.CardGames.FlipSeven;

public class FlipSevenCard : IFlipSevenCard
{
    public bool IsFaceDown => false;
    public int Value { get; init; }
    public string Name => GetName();
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

    public override string ToString()
        => Name;

    private string GetName()
        => CardType switch
        {
            CardType.Freeze => Constants.FreezeName,
            CardType.Flip => $"{Constants.FlipXName} {Value}",
            CardType.SecondChance => Constants.SecondChanceName,
            CardType.BonusAdd => $"+{Value}",
            CardType.Multiplier => $"x{Value}",
            CardType.Number => $"{Value}",
            _ => $"???"
        };
}
