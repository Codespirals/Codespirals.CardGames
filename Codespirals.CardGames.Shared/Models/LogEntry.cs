using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <inheritdoc cref="ILogEntry"/>
public record LogEntry : ILogEntry
{
    /// <inheritdoc/>
    public string Text { get; init; }
    /// <inheritdoc/>
    public int? Round { get; init; }
    /// <inheritdoc/>
    public int ActorId { get; init; }
    /// <inheritdoc cref="ILogEntry"/>
    public LogEntry(string text, int? round = null, int actorId = -1)
    {
        Text = text;
        Round = round;
        ActorId = actorId;
    }
}
