namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenDeck<TCard> : IDeck<TCard>
    where TCard : IFlipSevenCard
{
    public int Reshuffles { get; }
}
