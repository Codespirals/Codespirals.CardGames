using Codespirals.CardGames.Poker.BlackJack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.Poker;

internal static class PokerPlayerGenerator
{
    /// <inheritdoc/>
    public static BlackJackPlayer GenerateBlackJackDealer()
        => new BlackJackPlayer("Dealer", int.MaxValue);
    /// <inheritdoc/>
    public static BlackJackPlayer GenerateBlackJackPlayer(string name, int startingCash)
        => new BlackJackPlayer(name, startingCash);
    /// <inheritdoc/>
    public static BlackJackPlayer GenerateBlackJackPlayer(int number, int startingCash)
        => new BlackJackPlayer($"Player {number + 1}", startingCash);
}
