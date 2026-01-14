using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IGame<TSelf, TPlayer, TDeck, TCard>
    where TSelf : IGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public TDeck Deck { get; }
    public ReadOnlyCollection<TPlayer> Players { get; }
    public int CurrentRound { get; }
    public abstract static TSelf SetUp(int players);
}
