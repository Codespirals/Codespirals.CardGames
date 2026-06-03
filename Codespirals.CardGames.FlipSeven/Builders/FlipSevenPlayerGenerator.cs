using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.FlipSeven;

internal static class FlipSevenPlayerGenerator
{
    /// <inheritdoc />
    public static FlipSevenPlayer GeneratePlayer(string name)
        => new FlipSevenPlayer(name);
    /// <inheritdoc />
    public static FlipSevenPlayer GeneratePlayer(int number)
        => new FlipSevenPlayer($"Player {number}");
}
