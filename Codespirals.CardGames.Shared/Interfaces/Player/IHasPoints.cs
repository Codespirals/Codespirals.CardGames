using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

public interface IHasPoints
{
    int TotalPoints { get; }
    void AddWinnings(int amount);
}
