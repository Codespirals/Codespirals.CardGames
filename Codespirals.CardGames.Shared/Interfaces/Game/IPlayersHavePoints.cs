using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

public interface IPlayersHavePoints<TPlayer>
    where TPlayer : IPlayer, IHasPoints
{
    /// <summary>
    /// End the round and return information about the winnings of the round
    /// </summary>
    /// <returns></returns>
    (TPlayer Player, int Winnings)[] CalculateCurrentPotentialPointGain();
    public void PayOut();
}
