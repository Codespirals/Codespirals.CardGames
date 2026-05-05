namespace Codespirals.CardGames;
/// <summary>
/// A card game of some kind
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TDeck"></typeparam>
/// <typeparam name="TCard"></typeparam>
public interface IGame<TSelf, TDeck, TCard>
    where TSelf : IGame<TSelf, TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    /// <summary>
    /// The deck to play with in this card game
    /// </summary>
    TDeck Deck { get; }
    /// <summary>
    /// A flag that this game is over
    /// </summary>
    bool GameOver { get; }
    /// <summary>
    /// Set up this game with default parameters
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    static abstract TSelf SetUp(int players);
}
