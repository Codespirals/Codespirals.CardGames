using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;
/// <summary>
/// Players in this game have points that can be increased
/// </summary>
/// <typeparam name="TPlayer"></typeparam>
public interface IPlayersHavePoints<TPlayer>
    where TPlayer : IPlayer, IHasPoints
{
    /// <summary>
    /// End the round and return information about the winnings of the round
    /// </summary>
    /// <returns></returns>
    IEnumerable<(TPlayer Player, int Winnings)> CalculateCurrentPotentialPointGain();
    /// <summary>
    /// Increase the points of each player by how much they won this round
    /// </summary>
    public void PayOut();
}
