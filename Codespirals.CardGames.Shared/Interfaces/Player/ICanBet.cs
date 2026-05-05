namespace Codespirals.CardGames;

public interface ICanBet
{
    int Cash { get; }
    int CurrentBet { get; }
    bool TappedOut { get; }
    void Bet(int amount);
    void AddWinnings(int amount);
}
