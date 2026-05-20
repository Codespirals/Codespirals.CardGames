namespace Codespirals.CardGames.FlipSeven;

/// <summary>
/// A card in the game of Flip Seven
/// </summary>
public interface IFlipSevenCard : ICard
{
    /// <summary>
    /// The type of card this is
    /// </summary>
    CardType CardType { get; init; }
}
