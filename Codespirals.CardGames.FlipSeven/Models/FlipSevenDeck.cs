namespace Codespirals.CardGames.FlipSeven;

/// <summary>
/// A deck for the game of Flip "7"
/// </summary>
public class FlipSevenDeck : Deck<FlipSevenCard>
{
    /// <summary>
    /// The total count of number cards
    /// </summary>
    public int TotalNumberCards => StartingCards.Count(c => c.CardType is CardType.Number);
    /// <summary>
    /// Get the highest number card's value
    /// </summary>
    public int HighestNumberCard => StartingCards.Where(c => c.CardType is CardType.Number).Max(c => c.Value);
}
