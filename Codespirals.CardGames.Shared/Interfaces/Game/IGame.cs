namespace Codespirals.CardGames;
public interface IGame<TSelf, TDeck, TCard>
    where TSelf : IGame<TSelf, TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    TDeck Deck { get; }
    static abstract TSelf SetUp(int players);
    bool GameOver { get; }
}
