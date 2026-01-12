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
        => $"Flip 7 card: {Name}";

    private string GetName()
        => CardType switch
        {
            CardType.Freeze => Constants.FreezeName,
            CardType.FlipThree => Constants.FlipThreeName,
            CardType.SecondChance => Constants.SecondChanceName,
            CardType.BonusAdd => $"+{Value}",
            CardType.TimesTwo => $"X{Value}",
            CardType.Number => Value.ToString(),
            _ => $"+{Value}"
        };
}
