namespace Codespirals.CardGames;

public interface ICanBet : IHasPoints
{
    int CurrentBet { get; }
    bool TappedOut { get; }
    void Bet(int amount);
}
