namespace Codespirals.CardGames;
public interface ICard
{
    bool IsFaceDown { get; }
    int Value { get; }
    string Name { get; }
}
