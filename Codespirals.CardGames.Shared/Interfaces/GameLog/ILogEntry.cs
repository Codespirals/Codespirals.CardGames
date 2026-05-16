using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// A log entry
/// </summary>
public interface ILogEntry
{
    /// <summary>
    /// The text of a log entry
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// The round number
    /// </summary>
    public int? Round { get; }
}