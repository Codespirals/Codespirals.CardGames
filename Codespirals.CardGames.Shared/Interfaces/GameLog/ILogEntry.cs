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
    /// An id to identify who the log entry is about
    /// </summary>
    public int ActorId { get; }
    /// <summary>
    /// The text of a log entry
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// The round number
    /// </summary>
    public int? Round { get; }
}