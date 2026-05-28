using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// An activity log for a game
/// </summary>
/// <typeparam name="TEntry"></typeparam>
public interface IHasActivityLog<TEntry>
    where TEntry : ILogEntry
{
    /// <summary>
    /// The entries
    /// </summary>
    ReadOnlyCollection<TEntry> LogEntries { get; }

    /// <summary>
    /// Create a log entry
    /// </summary>
    /// <param name="text"></param>
    /// <param name="actorId"></param>
    void Log(string text, int? actorId = null);
}
