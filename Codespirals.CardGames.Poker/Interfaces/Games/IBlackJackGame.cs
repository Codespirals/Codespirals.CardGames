namespace Codespirals.CardGames.Poker;
public interface IBlackJackGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer, TDeck, TCard>
    where TSelf : IGame<TSelf, TDeck, TCard>
    where TPlayer : IPokerPlayer<TDeck, TCard>
    where TDeck : IPokerDeck<TCard>
    where TCard : IPokerCard
{
    int WinningScore { get; }
    int MinBet { get; }
    TCard Hit(TPlayer player);
    TCard DoubleDown(TPlayer player);
    void Stand(TPlayer player);
}
