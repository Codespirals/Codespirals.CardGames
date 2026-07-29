using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.FlipNumber;

internal static class FlipNumberPlayerGenerator
{
    /// <inheritdoc />
    public static FlipNumberPlayer GeneratePlayer(string name)
        => new FlipNumberPlayer(name);
    /// <inheritdoc />
    public static FlipNumberPlayer GeneratePlayer(int number)
        => new FlipNumberPlayer($"Player {number}");
}
