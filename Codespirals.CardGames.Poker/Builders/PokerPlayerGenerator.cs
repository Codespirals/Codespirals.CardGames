using Codespirals.CardGames.Poker.BlackJack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.Poker;

internal static class PokerPlayerGenerator
{
    /// <inheritdoc/>
    public static BlackJackPlayer GenerateBlackJackDealer(int maxScore = 21)
        => new BlackJackPlayer("Dealer", int.MaxValue, maxScore);
    /// <inheritdoc/>
    public static BlackJackPlayer GenerateBlackJackPlayer(string name, int startingCash, int maxScore = 21)
        => new BlackJackPlayer(name, startingCash, maxScore);
    /// <inheritdoc/>
    public static BlackJackPlayer GenerateBlackJackPlayer(int number, int startingCash, int maxScore = 21)
        => new BlackJackPlayer($"Player {number + 1}", startingCash, maxScore);
}
