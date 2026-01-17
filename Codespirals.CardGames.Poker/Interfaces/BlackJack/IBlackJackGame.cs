namespace Codespirals.CardGames.Poker.BlackJack;
public interface IBlackJackGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer, TDeck, TCard>, IHasBets
    where TSelf : IBlackJackGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IBlackJackPlayer<TDeck, TCard>
    where TDeck : IPokerDeck<TCard>
    where TCard : IPokerCard
{
    public TPlayer Dealer { get; }
    int WinningScore { get; }
    TCard Hit(TPlayer player);
    TCard DoubleDown(TPlayer player);
    void Stand(TPlayer player);
}
