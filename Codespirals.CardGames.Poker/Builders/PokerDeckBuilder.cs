namespace Codespirals.CardGames.Poker;
public class PokerDeckBuilder
{
    private readonly Deck _deck;

    private PokerDeckBuilder()
    {
        _deck = new Deck();
    }

    public static PokerDeckBuilder BeginBuilding()
        => new();

    public PokerDeckBuilder WithNumberCards(Suit[] suits, int min = 2, int max = 10)
    {
        foreach (var suit in suits)
        {
            for (var i = min; i < max; i++)
            {
                _deck.AddStartingCard(new Card(i.ToString(), i, suit));
            }
        }
        return this;
    }
    public PokerDeckBuilder WithNamedCards(Suit[] suits, NamedCards cardType, int value)
    {
        foreach (var suit in suits)
        {
            _deck.AddStartingCard(new Card(cardType, value, suit));
        }
        return this;
    }

    public PokerDeckBuilder WithExtraCards(ExtraCards cardType, int value, int number, Suit suit = Suit.Unknown)
    {
        for (var i = 0; i < number; i++)
        {
            _deck.AddStartingCard(new Card(cardType, value, suit));
        }
        return this;
    }

    public Deck Build()
    {
        _deck.OrderStartingCards();
        _deck.Reset();
        return _deck;
    }

    public Deck BuildMultiple(int number)
    {
        var temp = _deck;
        foreach (var card in temp.StartingCards)
        {
            for (var i = 0; i > number; i++)
            {
                _deck.AddStartingCard(card);
            }
        }
        _deck.OrderStartingCards();
        _deck.Reset();
        return _deck;
    }

    public static Deck BasicPokerDeck()
        => BeginBuilding()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Ace, 1)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Jack, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Queen, 12)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.King, 13)
        .Build();

    public static Deck BasicBlackJackDeck()
        => BeginBuilding()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Ace, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Jack, 10)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Queen, 10)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.King, 10)
        .Build();

    public static Deck BasicPokerDeckWithJokers(int jokers)
        => BeginBuilding()
        .WithNumberCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades])
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Ace, 1)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Jack, 11)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.Queen, 12)
        .WithNamedCards([Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades], NamedCards.King, 13)
        .WithExtraCards(ExtraCards.Joker, 0, jokers)
        .Build();

    public static Deck JassDeck()
        => BeginBuilding()
        .WithNumberCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], 6, 10)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Ace, 1)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Jack, 11)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Queen, 12)
        .WithNamedCards([Suit.Bells, Suit.Acorns, Suit.Flowers, Suit.Shields], NamedCards.Ace, 13)
        .Build();
}
