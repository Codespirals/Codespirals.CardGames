using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// A card implementing this interface can be face down
/// </summary>
public interface ICanBeFaceDown
{
    /// <summary>
    /// If this card is face down
    /// </summary>
    bool IsFaceDown { get; }
}
