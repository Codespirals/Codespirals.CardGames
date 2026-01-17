namespace Codespirals.CardGames;

public interface ICanBet
{
    public int Cash { get; }
    public int CurrentBet { get; }
    bool TappedOut { get; }
    public void Bet(int amount);
    public void AddWinnings(int amount);
}
