using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// This player has points or "cash"
/// </summary>
public interface IHasPoints
{
    /// <summary>
    /// The total points this player has
    /// </summary>
    int TotalPoints { get; }
    /// <summary>
    /// Add points to total
    /// </summary>
    /// <param name="amount"></param>
    void AddPoints(int amount);
}
