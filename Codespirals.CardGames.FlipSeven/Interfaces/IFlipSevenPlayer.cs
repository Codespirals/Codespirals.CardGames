namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>
    where TDeck : IFlipSevenDeck<TCard>
    where TCard : IFlipSevenCard
{
    public int Points { get; }
    public bool IsOutForRound { get; }
    public PlayerStates State { get; }
    public void BankPoints();
    public void Freeze();
    public void Bust();
    public void Reactivate();
}
