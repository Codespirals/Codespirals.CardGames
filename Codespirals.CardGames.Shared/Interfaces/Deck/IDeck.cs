using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
/// <summary>
/// A deck of <typeparamref name="TCard"/>s
/// </summary>
/// <typeparam name="TCard"></typeparam>
public interface IDeck<TCard>
    where TCard : ICard
{
    /// <summary>
    /// The full deck with all <typeparamref name="TCard"/>s
    /// </summary>
    ReadOnlyCollection<TCard> StartingCards { get; }
    /// <summary>
    /// The <typeparamref name="TCard"/>s that are still in the deck
    /// </summary>
    ReadOnlyCollection<TCard> CardPool { get; }
    /// <summary>
    /// The <typeparamref name="TCard"/>s that have been played and are no longer needed
    /// </summary>
    ReadOnlyCollection<TCard> DiscardPile { get; }
    /// <summary>
    /// When there are no more <typeparamref name="TCard"/>s in the deck, should the <see cref="DiscardPile"/> be shuffled into the <see cref="CardPool"/>
    /// </summary>
    public bool RefreshOnEmpty { get; }
    /// <summary>
    /// How many times this deck's <see cref="DiscardPile"/> has been shuffled into the <see cref="CardPool"/>
    /// </summary>
    public int Reshuffles { get; }

    /// <summary>
    /// Add a <typeparamref name="TCard"/> to the <see cref="StartingCards"/>
    /// </summary>
    /// <param name="card"></param>
    void AddStartingCard(TCard card);
    /// <summary>
    /// Draw a <typeparamref name="TCard"/> from the <see cref="CardPool"/>
    /// </summary>
    /// <returns></returns>
    TCard? Draw();
    /// <summary>
    /// Draw multiple <typeparamref name="TCard"/>s from the <see cref="CardPool"/>
    /// </summary>
    /// <param name="numberOfCards"></param>
    /// <returns></returns>
    IEnumerable<TCard> Draw(int numberOfCards);
    /// <summary>
    /// Peek at the top <paramref name="numberOfCards"/> of the <see cref="CardPool"/>
    /// </summary>
    /// <param name="numberOfCards"></param>
    /// <returns></returns>
    IEnumerable<TCard> Peek(int numberOfCards);
    /// <summary>
    /// Put a <typeparamref name="TCard"/> on the <see cref="DiscardPile"/>
    /// </summary>
    /// <param name="card"></param>
    void PutOnDiscardPile(TCard card);
    /// <summary>
    /// Put multiple <typeparamref name="TCard"/>s on the <see cref="DiscardPile"/>
    /// </summary>
    /// <param name="cards"></param>
    void PutOnDiscardPile(TCard[] cards);
    /// <summary>
    /// Shuffle the <see cref="DiscardPile"/> back into the <see cref="CardPool"/>
    /// </summary>
    void ReturnDiscardPileToCardPool();
    /// <summary>
    /// Shuffle the <see cref="CardPool"/>
    /// </summary>
    void Shuffle();
    /// <summary>
    /// Empty the <see cref="DiscardPile"/> and set the <see cref="CardPool"/> to the <see cref="StartingCards"/>
    /// </summary>
    void Reset();
}
