using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IGame<TSelf, TDeck, TCard, TPlayer>
    where TSelf : IGame<TSelf, TDeck, TCard, TPlayer>
    where TPlayer : IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public TDeck Deck { get; }
    public ReadOnlyCollection<TPlayer> Players { get; }
    public int RoundsPlayed { get; }
    public int CurrentRound { get; }
    public bool GameOver { get; }
    public abstract static TSelf SetUp(int players);
}
