using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.FlipSeven;

internal static class FlipSevenCardGenerator
{
    public static FlipSevenCard GenerateNumberCard(int value)
        => new FlipSevenCard(CardType.Number, Math.Abs(value));
    public static FlipSevenCard GenerateMultiplierCard(int value)
        => new FlipSevenCard(CardType.Multiplier, Math.Abs(value));
    public static FlipSevenCard GenerateBonusAddCard(int value)
        => new FlipSevenCard(CardType.BonusAdd, value);
    public static FlipSevenCard GenerateSecondChanceCard()
        => new FlipSevenCard(CardType.SecondChance);
    public static FlipSevenCard GenerateFlipCard(int value)
        => new FlipSevenCard(CardType.Flip, Math.Abs(value));
    public static FlipSevenCard GenerateFreezeCard()
        => new FlipSevenCard(CardType.Freeze);
}
