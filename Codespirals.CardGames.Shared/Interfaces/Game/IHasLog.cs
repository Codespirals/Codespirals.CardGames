using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

public interface IHasTextLog
{
    public IEnumerable<string> LogEntries { get; set; }
}
