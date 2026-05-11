namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>, ICanPlayRounds, ICanBust
    where TDeck : IDeck<TCard>
    where TCard : IFlipSevenCard
{
    int BankedPoints { get; }
    int HandPoints { get; }
    int NumberCardsInHand { get; }
    PlayerStates State { get; }
    void BankPoints();
    void Freeze();
}
