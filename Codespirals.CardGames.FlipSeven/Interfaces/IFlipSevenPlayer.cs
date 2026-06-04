namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc/>
public interface IFlipSevenPlayer<TCard> : IPlayer<TCard>, ICanPlayRounds, IHasPoints, ICanBust
    where TCard : IFlipSevenCard
{
    /// <summary>
    /// How many points this user has in their hands at this moment
    /// </summary>
    int HandPoints { get; }
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
