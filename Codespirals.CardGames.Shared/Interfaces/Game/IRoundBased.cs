namespace Codespirals.CardGames;
/// <summary>
/// This game is played in rounds
/// </summary>
public interface IRoundBased
{
    /// <summary>
    /// The number of the current round
    /// </summary>
    int CurrentRound { get; }
    /// <summary>
    /// Is a round active
    /// </summary>
    bool RoundActive { get; }
    /// <summary>
    /// Start a new round
    /// </summary>
    void StartRound();
    /// <summary>
    /// End the current round
    /// </summary>
    void EndRound();
}
