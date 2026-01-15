namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenDeck<TCard> : IDeck<TCard>
    where TCard : IFlipSevenCard
{
    int Reshuffles { get; }
}
