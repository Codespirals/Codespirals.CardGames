namespace Codespirals.CardGames.Poker.BlackJack;
public interface IBlackJackGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TPlayer, TDeck, TCard>
    where TSelf : IGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IPokerPlayer<TDeck, TCard>
    where TDeck : IPokerDeck<TCard>
    where TCard : IPokerCard
{
    public TCard Hit(TPlayer player);
    public TCard DoubleDown(TPlayer player);
    public void Stand(TPlayer player);
}
