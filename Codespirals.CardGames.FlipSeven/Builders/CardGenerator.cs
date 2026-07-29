using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.FlipNumber;

internal static class FlipNumberCardGenerator
{
    public static FlipNumberCard GenerateNumberCard(int value)
        => new FlipNumberCard(CardType.Number, Math.Abs(value));
    public static FlipNumberCard GenerateMultiplierCard(int value)
        => new FlipNumberCard(CardType.Multiplier, Math.Abs(value));
    public static FlipNumberCard GenerateBonusAddCard(int value)
        => new FlipNumberCard(CardType.BonusAdd, value);
    public static FlipNumberCard GenerateSecondChanceCard()
        => new FlipNumberCard(CardType.Protection);
    public static FlipNumberCard GenerateFlipCard(int value)
        => new FlipNumberCard(CardType.Flip, Math.Abs(value));
    public static FlipNumberCard GenerateFreezeCard()
        => new FlipNumberCard(CardType.ForceBank);
}
