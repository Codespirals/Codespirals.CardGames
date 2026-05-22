using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.Uno;

public class UnoCard : IUnoCard
{
    public CardType CardType { get; private set; }

    public Color Color { get; private set; }

    public int Value { get; init; }

    public string Name => GetName();

    private UnoCard(CardType cardType, Color color = Color.None, int value = 0)
    {
        CardType = cardType;
        Value = value;
        Color = color;
    }
    public static UnoCard GenerateNumberCard(int value, Color color)
        => new UnoCard(CardType.Number, color, value);
    public static UnoCard GenerateDrawCard(int value, Color color)
        => new UnoCard(CardType.Draw, color, value);
    public static UnoCard GenerateReverseCard(Color color)
        => new UnoCard(CardType.Reverse, color);
    public static UnoCard GenerateSkipCard(Color color)
        => new UnoCard(CardType.Skip, color);
    public static UnoCard GenerateChooseCard()
        => new UnoCard(CardType.Choose);
    public static UnoCard GenerateDrawAndChooseCard(int value)
        => new UnoCard(CardType.DrawAndChoose, value:value);

    private string GetName()
        => CardType switch
        {
            CardType.Number => $"{Color.ToString()} {Value}",
            CardType.Draw => $"{Color.ToString()} +{Value}",
            CardType.Skip => $"{Color.ToString()} {UnoConstants.SKIPNAME}",
            CardType.Reverse => $"{Color.ToString()} {UnoConstants.REVERSENAME}",
            CardType.Choose => $"{UnoConstants.CHOOSENAME}",
            CardType.DrawAndChoose => $"{UnoConstants.CHOOSENAME} +{Value}",
            _ => $"???"
        };
}
