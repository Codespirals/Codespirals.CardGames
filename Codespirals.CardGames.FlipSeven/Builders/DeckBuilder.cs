namespace Codespirals.CardGames.FlipNumber;

/// <inheritdoc/>
public class FlipNumberDeckBuilder : IDeckBuilder<FlipNumberDeckBuilder, FlipNumberDeck, FlipNumberCard>
{
    private readonly FlipNumberDeck _deck;
    private FlipNumberDeckBuilder()
    {
        _deck = new FlipNumberDeck();
    }
    /// <inheritdoc/>
    public static FlipNumberDeckBuilder Begin()
        => new();

    /// <inheritdoc/>
    public FlipNumberDeckBuilder RefreshOnEmpty(bool refresh = true)
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
    /// <remarks>Min:2, Max:255</remarks>
    public FlipNumberDeckBuilder WithNumberCards(int highestNumber = 12)
    {
        _deck.AddStartingCard(FlipNumberCardGenerator.GenerateNumberCard(0));
        highestNumber = Math.Clamp(highestNumber, 2, byte.MaxValue);
        for (var v = highestNumber; v > 0; v--)
        {
            for (var i = 0; i < v; i++)
            {
                _deck.AddStartingCard(FlipNumberCardGenerator.GenerateNumberCard(v));
            }
        }
        return this;
    }
    /// <summary>
    /// Adds additive (+X) bonus cards to your deck
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <remarks>Min:1, Max:255</remarks>
    public FlipNumberDeckBuilder WithBonusCards(int[] values)
    {
        foreach (var value in values)
        {
            _deck.AddStartingCard(FlipNumberCardGenerator.GenerateBonusAddCard(Math.Clamp(value, 1, byte.MaxValue)));
        }
        return this;
    }
    /// <summary>
    /// Adds multiplier (*X) bonus cards to your deck
    /// </summary>
    /// <param name="number"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public FlipNumberDeckBuilder WithMultipliers(int number = 1, int multiplier = 2)
    {
        number = Math.Clamp(number, 1, byte.MaxValue);
        multiplier = Math.Clamp(multiplier, 2, byte.MaxValue);
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipNumberCardGenerator.GenerateMultiplierCard(multiplier));
        }
        return this;
    }
    /// <summary>
    /// Adds "FlipX" cards to your deck that force a player to draw X cards
    /// </summary>
    /// <param name="number"></param>
    /// <param name="draw"></param>
    /// <returns></returns>
    public FlipNumberDeckBuilder WithFlipCards(int number = 3, int draw = 3)
    {
        number = Math.Clamp(number, 1, byte.MaxValue);
        draw = Math.Clamp(draw, 1, byte.MaxValue);
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipNumberCardGenerator.GenerateFlipCard(draw));
        }
        return this;
    }
    /// <summary>
    /// Adds Freeze cards to your deck that force a player to bank for the current round
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public FlipNumberDeckBuilder WithFreezes(int number = 3)
    {
        number = Math.Clamp(number, 1, byte.MaxValue);
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipNumberCardGenerator.GenerateFreezeCard());
        }
        return this;
    }
    /// <summary>
    /// Adds second chances to your deck that protect you from busting once
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public FlipNumberDeckBuilder WithSecondChances(int number = 3)
    {
        number = Math.Clamp(number, 1, byte.MaxValue);
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipNumberCardGenerator.GenerateSecondChanceCard());
        }
        return this;
    }
    /// <inheritdoc/>
    public FlipNumberDeck Build()
    {
        _deck.Reset();
        return _deck;
    }

    /// <inheritdoc/>
    public static FlipNumberDeck CreateStandardDeck()
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
