namespace Codespirals.CardGames.Poker;
public interface IPokerDeck<TCard> : IDeck<TCard>
    where TCard : IPokerCard
{
}
