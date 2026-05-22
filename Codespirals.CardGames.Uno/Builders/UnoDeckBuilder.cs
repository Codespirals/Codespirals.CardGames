namespace Codespirals.CardGames.Uno;

/// <inheritdoc/>
public class UnoDeckBuilder : IDeckBuilder<UnoDeckBuilder, UnoDeck, UnoCard>
{
    private readonly UnoDeck _deck;
    private UnoDeckBuilder()
    {
        _deck = new UnoDeck();
    }
    /// <inheritdoc/>
    public static UnoDeckBuilder Begin()
        => new();

    /// <inheritdoc/>
    public UnoDeckBuilder RefreshOnEmpty(bool refresh = true)
    {
        _deck.RefreshOnEmpty = true;
        return this;
    }
    /// <summary>
    /// Adds number cards to the deck in the chosen color, starting from lowest and to highest
    /// </summary>
    /// <returns></returns>
    public UnoDeckBuilder WithNumberCards(Color color, int lowest = 0, int highest = 9)
    {
        for (var v = lowest; v > highest; v--)
        {
            _deck.AddStartingCard(UnoCard.GenerateNumberCard(v, color));
        }
        return this;
    }
    /// <summary>
    /// Adds number cards to the deck in the chosen color, starting from lowest and to highest
    /// </summary>
    /// <returns></returns>
    public UnoDeckBuilder WithNumberCardsInAllColors(int lowest = 0, int highest = 9)
    {
        for (int i = (int)Color.Red; i < (int)Color.Yellow; i++)
        {
            this.WithNumberCards((Color)i, lowest, highest);
        }
        return this;
    }
    public UnoDeckBuilder WithReverseCards(Color color, int number = 2)
    {
        for (var v = number; v > 0; v--)
        {
            _deck.AddStartingCard(UnoCard.GenerateReverseCard(color));
        }
        return this;
    }
    public UnoDeckBuilder WithReverseCardsInAllColors(int number = 2)
    {
        for (int i = (int)Color.Red; i < (int)Color.Yellow; i++)
        {
            this.WithReverseCards((Color)i, number);
        }
        return this;
    }
    public UnoDeckBuilder WithSkipCards(Color color, int number = 2)
    {
        for (var v = number; v > 0; v--)
        {
            _deck.AddStartingCard(UnoCard.GenerateSkipCard(color));
        }
        return this;
    }
    public UnoDeckBuilder WithSkipCardsAllColors(int number = 2)
    {
        for (int i = (int)Color.Red; i < (int)Color.Yellow; i++)
        {
            this.WithSkipCards((Color)i, number);
        }
        return this;
    }
    public UnoDeckBuilder WithDrawCards(Color color, int number = 2, int draw = 2)
    {
        for (var v = number; v > 0; v--)
        {
            _deck.AddStartingCard(UnoCard.GenerateDrawCard(draw, color));
        }
        return this;
    }
    public UnoDeckBuilder WithDrawCardsAllColors(int number = 2, int draw = 2)
    {
        for (int i = (int)Color.Red; i < (int)Color.Yellow; i++)
        {
            this.WithDrawCards((Color)i, number, draw);
        }
        return this;
    }
    public UnoDeckBuilder WithChooseCards(int number = 4)
    {
        for (int i = 0; i < number; i++)
        {
            _deck.AddStartingCard(UnoCard.GenerateChooseCard());
        }
        return this;
    }
    public UnoDeckBuilder WithChooseAndDrawCards(int number = 4, int draw = 4)
    {
        for (int i = 0; i < number; i++)
        {
            _deck.AddStartingCard(UnoCard.GenerateChooseCard());
        }
        return this;
    }
    /// <inheritdoc/>
    public UnoDeck Build()
    {
        _deck.Reset();
        return _deck;
    }

    /// <inheritdoc/>
    public static UnoDeck CreateStandardDeck()
        => Begin()
        .RefreshOnEmpty()
        .WithNumberCardsInAllColors()
        .WithNumberCardsInAllColors(1, 9)
        .WithSkipCardsAllColors()
        .WithReverseCardsInAllColors()
        .WithDrawCardsAllColors()
        .WithChooseCards()
        .WithChooseCards()
        .Build();
}
