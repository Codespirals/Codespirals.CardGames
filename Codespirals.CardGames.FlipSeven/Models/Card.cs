namespace Codespirals.CardGames.FlipSeven;

public class Card : IFlipSevenCard
{
    public bool IsFaceDown => false;
    public int Value { get; init; }
    public string Name => GetName();
    public CardType CardType { get; init; }

    public Card(CardType cardType, int value = 0)
    {
        CardType = cardType;
        Value = value;
    }

    public override string ToString()
        => Name;

    private string GetName()
        => CardType switch
        {
            CardType.Freeze => Constants.FreezeName,
            CardType.Flip => $"{Constants.FlipXName} {Value}",
            CardType.SecondChance => Constants.SecondChanceName,
            CardType.BonusAdd => $"+{Value}",
            CardType.Multiplier => $"X{Value}",
            CardType.Number => Value.ToString(),
            _ => $"Flip Seven Card"
        };
}
