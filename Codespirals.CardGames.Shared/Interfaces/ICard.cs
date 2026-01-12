namespace Codespirals.CardGames;
public interface ICard
{
    public bool IsFaceDown { get; }
    public int Value { get; }
    public string Name { get; }
}
