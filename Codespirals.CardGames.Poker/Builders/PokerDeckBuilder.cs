namespace Codespirals.CardGames.Poker;
public class PokerDeckBuilder
{
    private PokerDeck _deck;

    private PokerDeckBuilder()
    {
        _deck = new PokerDeck();
    }

    public static PokerDeckBuilder BeginBuilding()
        => new();

    public PokerDeckBuilder RefreshOnEmpty(bool refresh)
    {
        _deck.RefreshOnEmpty = refresh;
        return this;
    }

    public PokerDeckBuilder WithNumberCards(Suit[] suits, int min = 2, int max = 10)
    {
        foreach (var suit in suits)
        {
            for (var i = min; i < max; i++)
            {
                _deck.AddStartingCard(PokerCard.GenerateNumberCard(i.ToString(), i, suit));
            }
        }
        return this;
    }
    public PokerDeckBuilder WithNamedCards(Suit[] suits, NamedCards cardType, int value)
    {
        foreach (var suit in suits)
        {
            _deck.AddStartingCard(PokerCard.GenerateNamedCard(cardType, value, suit));
        }
        return this;
    }

    public PokerDeckBuilder WithExtraCards(ExtraCards cardType, int value, int number, Suit suit = Suit.Unknown)
    {
        for (var i = 0; i < number; i++)
        {
            _deck.AddStartingCard(PokerCard.GenerateExtraCard(cardType, value, suit));
        }
        return this;
    }

    public PokerDeck Build()
    {
        _deck.Reset();
        return _deck;
    }

    public PokerDeck BuildMultiple(int number)
    {
        var temp = _deck.StartingCards.ToList().AsReadOnly();
        for (var i = 0; i > number; i++)
        {
            _deck.StartingCards.Concat(temp);
        }
        _deck.Reset();
        return _deck;
    }

    public static PokerDeck BasicPokerDeck()
        => BeginBuilding()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Ace, 1)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Jack, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Queen, 12)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.King, 13)
        .RefreshOnEmpty(true)
        .Build();

    public static PokerDeck BasicBlackJackDeck()
        => BeginBuilding()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Ace, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Jack, 10)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Queen, 10)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.King, 10)
        .RefreshOnEmpty(true)
        .Build();

    public static PokerDeck BasicPokerDeckWithJokers(int jokers)
        => BeginBuilding()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Ace, 1)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Jack, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Queen, 12)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.King, 13)
        .WithExtraCards(ExtraCards.Joker, 0, jokers)
        .RefreshOnEmpty(true)
        .Build();

    public static PokerDeck JassDeck()
        => BeginBuilding()
        .WithNumberCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], 6, 10)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Ace, 1)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Jack, 11)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Queen, 12)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Ace, 13)
        .RefreshOnEmpty(true)
        .Build();
}
