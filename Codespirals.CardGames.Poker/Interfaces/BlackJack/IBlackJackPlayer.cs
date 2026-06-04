namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc/>
public interface IBlackJackPlayer<TCard, TDeck> : IPlayer<TCard>, ICanPlayRounds, ICanBet, ICanBust, IHasPoints
    where TCard : IPokerCard
{
    /// <summary>
    /// The state of the player
    /// </summary>
    public PlayerState State { get; }
    /// <summary>
    /// The player stands on their current hand
    /// </summary>
    public void Stand();
}
