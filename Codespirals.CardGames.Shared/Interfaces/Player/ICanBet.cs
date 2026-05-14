namespace Codespirals.CardGames;

/// <summary>
/// Players can bet
/// </summary>
public interface ICanBet : IHasPoints
{
    /// <summary>
    /// How much the player has currently bet
    /// </summary>
    int CurrentBet { get; }
    /// <summary>
    /// If this player is completely out of cash
    /// </summary>
    bool TappedOut { get; }
    /// <summary>
    /// Increase <see cref="CurrentBet"/>
    /// </summary>
    /// <param name="amount"></param>
    void Bet(int amount);
}
