using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IHasPlayers<TPlayer, TDeck, TCard>
    where TPlayer : IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    ReadOnlyCollection<TPlayer> Players { get; }
    TPlayer GetCurrentPlayer();
    void MoveToNextPlayer();
}
