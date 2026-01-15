using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer, TDeck, TCard>
    where TSelf : IFlipSevenGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IFlipSevenPlayer<TDeck, TCard>
    where TDeck : IFlipSevenDeck<TCard>
    where TCard : IFlipSevenCard
{
    ReadOnlyCollection<Player> ActivePlayers { get; }
    int WinningScore { get; }
    int NumbersToFlip { get; }

    TCard Flip(TPlayer player);
    void Freeze(TPlayer player);
    Player? GetWinner();
}
