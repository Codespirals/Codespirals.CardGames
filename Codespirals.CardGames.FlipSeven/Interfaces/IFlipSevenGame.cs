using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer, TDeck, TCard>
    where TSelf : IFlipSevenGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IFlipSevenPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : IFlipSevenCard
{
    abstract static FlipSevenGame SetUp(int players, int numbersToFlip = 7, int winningScore = 200, FlipSevenDeck? deck = null);
    int WinningScore { get; }
    int NumbersToFlip { get; }

    TCard? Flip(TPlayer player);
    IEnumerable<TCard> Flip(TPlayer player, int number);
    void Freeze(TPlayer player);
    TPlayer? GetWinner();
}
