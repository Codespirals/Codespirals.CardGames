namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>, ICanPlayRounds, IHasPoints
    where TDeck : IDeck<TCard>
    where TCard : IFlipSevenCard
{
    int HandPoints { get; }
    int NumberCardsInHand { get; }
    PlayerStates State { get; }
    int Bank();
    int Freeze();
}
