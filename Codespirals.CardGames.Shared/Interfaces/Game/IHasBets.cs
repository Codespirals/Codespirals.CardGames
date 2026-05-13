namespace Codespirals.CardGames;

public interface IHasBets<TPlayer> : IPlayersHavePoints<TPlayer>
    where TPlayer : IPlayer, IHasPoints
{
    int BuyIn { get; }
    public void RaiseTheStakes(int amount);
    public void PayOut();
}
