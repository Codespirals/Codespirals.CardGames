namespace Codespirals.CardGames;
/// <summary>
/// This player can play in rounds
/// </summary>
public interface ICanPlayRounds
{
    /// <summary>
    /// This player is out for the current round
    /// </summary>
    bool IsOutForRound { get; }
    /// <summary>
    /// Deactivate this player for the current round
    /// </summary>
    void DeactivateForRound();
    /// <summary>
    /// Reactivate this player
    /// </summary>
    void Reactivate();
}
