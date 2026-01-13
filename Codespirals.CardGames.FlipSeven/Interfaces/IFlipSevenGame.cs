using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public interface IFlipSevenGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TPlayer, TDeck, TCard>
    where TSelf : IFlipSevenGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IFlipSevenPlayer<TDeck, TCard>
    where TDeck : IFlipSevenDeck<TCard>
    where TCard : IFlipSevenCard
{
    public ReadOnlyCollection<Player> ActivePlayers {  get; }
    public int WinningScore { get; }
    public int NumbersToFlip { get; }
    public bool RoundActive { get; }

    public void StartRound();
    public Player GetCurrentPlayer();
    public void MoveToNextPlayer();
    public TCard Flip(TPlayer player);
    public void Freeze(TPlayer player);
    public void EndRound();
    public Player? GetWinner();
}
