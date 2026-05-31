using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.Poker;

internal static class PokerCardGenerator
{
    public static PokerCard GenerateNumberCard(string name, int value, Suit suit)
        => new PokerCard(name, value, suit);
    public static PokerCard GenerateNamedCard(NamedCard cardType, int value, Suit suit)
        => new PokerCard(cardType, value, suit);
    public static PokerCard GenerateExtraCard(ExtraCard cardType, int value, Suit suit)
        => new PokerCard(cardType, value, suit);
}
