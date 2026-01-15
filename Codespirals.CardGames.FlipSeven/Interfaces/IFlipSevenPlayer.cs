namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>, ICanPlayRounds
    where TDeck : IFlipSevenDeck<TCard>
    where TCard : IFlipSevenCard
{
    int BankedPoints { get; }
    int HandPoints { get; }
    PlayerStates State { get; }
    TCard Flip(Card card);
    void BankPoints();
    void Freeze();
}
