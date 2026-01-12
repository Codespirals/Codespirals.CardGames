namespace Codespirals.CardGames;
public interface ICard
{
    public bool IsFaceDown { get; }
    public string Name { get; }
    public int Value { get; }
}
