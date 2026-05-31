namespace Codespirals.CardGames.Poker;

/// <inheritdoc cref="IDeckBuilder{TSelf, TDeck, TCard}"/>
public class PokerDeckBuilder : IDeckBuilder<PokerDeckBuilder, PokerDeck, PokerCard>
{
    private PokerDeck _deck;

    private PokerDeckBuilder()
    {
        _deck = new PokerDeck();
    }

    /// <inheritdoc/>
    public static PokerDeckBuilder Begin()
        => new();

    /// <inheritdoc/>
    public PokerDeckBuilder RefreshOnEmpty(bool refresh)
    {
        _deck.RefreshOnEmpty = refresh;
        return this;
    }

    /// <summary>
    /// Add number cards in the chosen suits to the deck
    /// </summary>
    /// <param name="suits"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public PokerDeckBuilder WithNumberCards(Suit[] suits, int min = 2, int max = 10)
    {
        min = Math.Clamp(min, 1, byte.MaxValue);
        max = Math.Clamp(max, min, byte.MaxValue);
        foreach (var suit in suits)
        {
            for (var i = min; i < max; i++)
            {
                _deck.AddStartingCard(PokerCardGenerator.GenerateNumberCard(i.ToString(), i, suit));
            }
        }
        return this;
    }

    /// <summary>
    /// Add "named" cards like <see cref="NamedCard.King"/> or <see cref="NamedCard.Queen"/>
    /// </summary>
    /// <param name="suits"></param>
    /// <param name="cardType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public PokerDeckBuilder WithNamedCards(Suit[] suits, NamedCard cardType, int value)
    {
        value = Math.Clamp(value, 1, short.MaxValue);
        foreach (var suit in suits)
        {
            _deck.AddStartingCard(PokerCardGenerator.GenerateNamedCard(cardType, value, suit));
        }
        return this;
    }

    /// <summary>
    /// Add special extra cards to the deck like <see cref="ExtraCard.Joker"/>
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="value"></param>
    /// <param name="number"></param>
    /// <param name="suit"></param>
    /// <returns></returns>
    public PokerDeckBuilder WithExtraCards(ExtraCard cardType, int value, int number, Suit suit = Suit.Unknown)
    {
        number = Math.Clamp(number, 1, byte.MaxValue);
        value = Math.Clamp(value, 1, short.MaxValue);
        for (var i = 0; i < number; i++)
        {
            _deck.AddStartingCard(PokerCardGenerator.GenerateExtraCard(cardType, value, suit));
        }
        return this;
    }

    /// <inheritdoc/>
    public PokerDeck Build()
    {
        _deck.Reset();
        return _deck;
    }

    /// <summary>
    /// Multiply all cards in the deck by <paramref name="number"/>
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public PokerDeck BuildMultiple(int number)
    {
        number = Math.Clamp(number, 1, byte.MaxValue);
        var temp = _deck.StartingCards.ToList().AsReadOnly();
        for (var i = 0; i > number; i++)
        {
            _deck.StartingCards.Concat(temp);
        }
        _deck.Reset();
        return _deck;
    }

    /// <inheritdoc/>
    public static PokerDeck CreateStandardDeck()
        => Begin()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Ace, 1)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Jack, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Queen, 12)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.King, 13)
        .RefreshOnEmpty(true)
        .Build();

    /// <summary>
    /// Create a basic BlackJack deck
    /// </summary>
    /// <returns></returns>
    public static PokerDeck CreateBlackJackDeck()
        => Begin()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Ace, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Jack, 10)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Queen, 10)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.King, 10)
        .RefreshOnEmpty(true)
        .Build();

    /// <summary>
    /// Create a basic Poker deck with added <see cref="ExtraCard.Joker"/>s
    /// </summary>
    /// <param name="jokers"></param>
    /// <returns></returns>
    public static PokerDeck CreatePokerDeckWithJokers(int jokers)
        => Begin()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Ace, 1)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Jack, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.Queen, 12)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCard.King, 13)
        .WithExtraCards(ExtraCard.Joker, 0, jokers)
        .RefreshOnEmpty(true)
        .Build();

    /// <summary>
    /// Create a deck for the Swiss game of "Jass"
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/Jass</remarks>
    /// <returns></returns>
    public static PokerDeck CreateJassDeck()
        => Begin()
        .WithNumberCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], 6, 10)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCard.Jack, 11)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCard.Queen, 12)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCard.King, 13)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCard.Ace, 14)
        .RefreshOnEmpty(true)
        .Build();
}
