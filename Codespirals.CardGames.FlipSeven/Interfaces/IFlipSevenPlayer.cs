namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>
    where TDeck : IFlipSevenDeck<TCard>
    where TCard : IFlipSevenCard
{
    public int Points { get; }
    public bool IsOutForRound { get; }
    public PlayerStates State { get; }
    public void BankPoints(TDeck deck, int numberToFlip = 7);
    public void Freeze(TDeck deck);
    public void Bust(TDeck deck);
    public void Reactivate(TDeck deck);
}
