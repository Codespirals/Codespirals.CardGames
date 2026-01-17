namespace Codespirals.CardGames;

public interface IHasBets
{
    int BuyIn { get; }
    public void RaiseTheStakes(int amount);
}
