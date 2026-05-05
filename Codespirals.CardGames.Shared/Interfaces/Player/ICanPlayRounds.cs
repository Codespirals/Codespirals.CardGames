namespace Codespirals.CardGames;
public interface ICanPlayRounds
{
    bool IsOutForRound { get; }
    void DeactivateForRound();
    void Reactivate();
}
