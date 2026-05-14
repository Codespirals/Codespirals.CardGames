namespace Codespirals.CardGames;
/// <summary>
/// This game has bets players can place
/// </summary>
/// <typeparam name="TPlayer"></typeparam>
public interface IHasBets<TPlayer> : IPlayersHavePoints<TPlayer>
    where TPlayer : IPlayer, IHasPoints
{
    /// <summary>
    /// How much a player has to pay to play in a round
    /// </summary>
    int BuyIn { get; }
    /// <summary>
    /// Increase the <see cref="BuyIn"/>
    /// </summary>
    /// <param name="amount"></param>
    public void RaiseTheStakes(int amount);
}
