using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// This player can be "busted" out of the game
/// </summary>
public interface ICanBust
{
    /// <summary>
    /// This player is out of the game or round
    /// </summary>
    bool IsBusted { get; }
    /// <summary>
    /// Make this player bust
    /// </summary>
    void Bust();
}
