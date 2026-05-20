namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc/>
public class FlipSevenDeckBuilder : IDeckBuilder<FlipSevenDeckBuilder, FlipSevenDeck, FlipSevenCard>
{
    private readonly FlipSevenDeck _deck;
    private FlipSevenDeckBuilder()
    {
        _deck = new FlipSevenDeck();
    }
    /// <inheritdoc/>
    public static FlipSevenDeckBuilder Begin()
        => new();

    /// <inheritdoc/>
    public FlipSevenDeckBuilder RefreshOnEmpty(bool refresh = true)
    {
        _deck.RefreshOnEmpty = true;
        return this;
    }
    /// <summary>
    /// This adds number cards to your deck - starting from your selected <paramref name="highestNumber"/> counting down
    /// this adds X cards of that number to your deck, where X is the current number
    /// </summary>
    /// <param name="highestNumber"></param>
    /// <returns></returns>
    public FlipSevenDeckBuilder WithNumberCards(int highestNumber = 12)
    {
        _deck.AddStartingCard(FlipSevenCard.GenerateNumberCard(0));
        for (var v = highestNumber; v > 0; v--)
        {
            for (var i = 0; i < v; i++)
            {
                _deck.AddStartingCard(FlipSevenCard.GenerateNumberCard(v));
            }
        }
        return this;
    }
    /// <summary>
    /// Adds additive (+X) bonus cards to your deck
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public FlipSevenDeckBuilder WithBonusCards(int[] values)
    {
        foreach (var value in values)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateBonusAddCard(value));
        }
        return this;
    }
    /// <summary>
    /// Adds multiplier (*X) bonus cards to your deck
    /// </summary>
    /// <param name="number"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public FlipSevenDeckBuilder WithMultipliers(int number = 1, int multiplier = 2)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateMultiplierCard(multiplier));
        }
        return this;
    }
    /// <summary>
    /// Adds "FlipX" cards to your deck that force a player to draw X cards
    /// </summary>
    /// <param name="number"></param>
    /// <param name="draw"></param>
    /// <returns></returns>
    public FlipSevenDeckBuilder WithFlipCards(int number = 3, int draw = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateFlipCard(draw));
        }
        return this;
    }
    /// <summary>
    /// Adds Freeze cards to your deck that force a player to bank for the current round
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public FlipSevenDeckBuilder WithFreezes(int number = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateFreezeCard());
        }
        return this;
    }
    /// <summary>
    /// Adds second chances to your deck that protect you from busting once
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public FlipSevenDeckBuilder WithSecondChances(int number = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateSecondChanceCard());
        }
        return this;
    }
    /// <inheritdoc/>
    public FlipSevenDeck Build()
    {
        _deck.Reset();
        return _deck;
    }

    /// <inheritdoc/>
    public static FlipSevenDeck CreateStandardDeck()
        => Begin()
        .RefreshOnEmpty()
        .WithNumberCards()
        .WithBonusCards([2,4,6,8,10])
        .WithFlipCards()
        .WithMultipliers()
        .WithFreezes()
        .WithSecondChances()
        .Build();
}
