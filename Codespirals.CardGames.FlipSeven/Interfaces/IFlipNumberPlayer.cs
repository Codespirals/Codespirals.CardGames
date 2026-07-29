namespace Codespirals.CardGames.FlipNumber;

/// <summary>
/// A player of the game Flip Number
/// </summary>
/// <typeparam name="TCard"></typeparam>
public interface IFlipNumberPlayer<TCard> : IPlayer<TCard>, ICanPlayRounds, IHasPoints, ICanBust
    where TCard : IFlipNumberCard
{
    /// <summary>
    /// How many <see cref="CardType.Number"/> <typeparamref name="TCard"/>s this user currently has
    /// </summary>
    int NumberCardsInHand { get; }
    /// <summary>
    /// The player state
    /// </summary>
    PlayerState State { get; }
    /// <summary>
    /// The player adds their current <see cref="HandPoints"/> to their <see cref="IHasPoints.TotalPoints"/> and is out for the round
    /// </summary>
    /// <returns></returns>
    void Bank();
    /// <summary>
    /// The player is forced to <see cref="Bank"/>
    /// </summary>
    /// <returns></returns>
    void Freeze();
}
