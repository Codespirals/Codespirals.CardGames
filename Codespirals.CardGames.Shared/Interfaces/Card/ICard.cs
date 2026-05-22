namespace Codespirals.CardGames;
/// <summary>
/// A playing card of some kind
/// </summary>
public interface ICard
{
    /// <summary>
    /// The value of this card
    /// </summary>
    int Value { get; }
    /// <summary>
    /// The name of this card
    /// </summary>
    string Name { get; }
}
