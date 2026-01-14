namespace Codespirals.CardGames.Poker;
public interface IPokerPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>
    where TDeck : IPokerDeck<TCard>
    where TCard : IPokerCard 
{
    public int Points { get; }
    public int CurrentBet { get; }

    public void Bet(int amount);
}
