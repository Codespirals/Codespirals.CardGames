namespace Codespirals.CardGames;
public interface IRoundBased
{
    int CurrentRound { get; }
    bool RoundActive { get; }
    void StartRound();
    void EndRound();
}
