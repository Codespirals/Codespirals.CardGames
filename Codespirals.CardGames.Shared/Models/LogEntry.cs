using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <inheritdoc/>
public record LogEntry(string text, int? round = null) : ILogEntry
{
    /// <inheritdoc/>
    public string Text { get; } = text;
    /// <inheritdoc/>
    public int? Round { get; } = round;
}
