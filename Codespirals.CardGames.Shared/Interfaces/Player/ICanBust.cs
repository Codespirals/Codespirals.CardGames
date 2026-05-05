using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

public interface ICanBust
{
    bool IsBusted { get; }
    void Bust();
}
