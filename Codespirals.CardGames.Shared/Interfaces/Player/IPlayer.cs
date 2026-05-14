using System.Collections.ObjectModel;

namespace Codespirals.CardGames;

/// <summary>
/// A player in a game
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// The name of this player
    /// </summary>
    string Name { get; set; }
}
/// <summary>
/// A player with a hand full of cards
/// </summary>
/// <typeparam name="TCard"></typeparam>
public interface IPlayer<TCard> : IPlayer
    where TCard : ICard
{
    /// <summary>
    /// The hand of this player
    /// </summary>
    ReadOnlyCollection<TCard> Hand { get; }

    /// <summary>
    /// Add a card to <see cref="Hand"/>
    /// </summary>
    /// <param name="card"></param>
    void AddCardToHand(TCard card);
    /// <summary>
    /// Remove a card from the <see cref="Hand"/>
    /// </summary>
    /// <param name="card"></param>
    void Discard(TCard card);
    /// <summary>
    /// Empty the <see cref="Hand"/>
    /// </summary>
    void DiscardAll();
}