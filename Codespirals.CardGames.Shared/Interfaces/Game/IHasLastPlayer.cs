using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

public interface IHasLastPlayer<TPlayer>
    where TPlayer : IPlayer
{
    public TPlayer? LastPlayer { get; }
}
