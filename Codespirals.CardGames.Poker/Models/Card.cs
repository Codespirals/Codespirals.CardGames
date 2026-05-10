namespace Codespirals.CardGames.Poker;

public class Card : IPokerCard
{
    private readonly string? _emoji;
    private readonly int _value = 0;
    private readonly Suit _suit = Suit.Unknown;
    private readonly string _name = "";
    public int Value => _value;
    public string Name => GetName();
    public Suit Suit => IsFaceDown ? Suit.Unknown : _suit;
    public string? Emoji => GetSymbol();
    public bool IsFaceDown { get; internal set; }

    private Card(string name, int value, Suit suit)
    {
        _suit = suit;
        _name = name;
        _value = value;

        // if the card is of traditinal suits, there's an emoji for it
        if (_suit is >= Suit.Spades and <= Suit.Clubs)
            _emoji = char.ConvertFromUtf32(Convert.ToInt32("01F0A0", 16) + (((int)_suit - 1) * 16) + _value);
    }

    private Card(NamedCards cardType, int value, Suit suit)
    {
        _suit = suit;
        _name = cardType.ToString();
        _value = value;

        // if the card is of traditinal suits, there's an emoji for it
        if (_suit is >= Suit.Spades and <= Suit.Clubs)
            _emoji = char.ConvertFromUtf32(Convert.ToInt32("01F0A0", 16) + (((int)_suit - 1) * 16) + (int)cardType);
    }

    private Card(ExtraCards cardType, int value, Suit suit = Suit.Unknown)
    {
        _suit = suit;
        _value = value;
        // joker
        if (cardType is ExtraCards.Joker)
        {
            _name = ExtraCards.Joker.ToString();
            _emoji = PokerConstants.JOKEREMOJI;
        }
        // fool
        else if (cardType is ExtraCards.Fool)
        {
            _name = ExtraCards.Fool.ToString();
            _emoji = PokerConstants.FOOLEMOJI;
        }
        else
        {
            _name = "";
        }
    }

    public static Card GenerateNumberCard(string name, int value, Suit suit)
        => new(name, value, suit);
    public static Card GenerateNamedCard(NamedCards cardType, int value, Suit suit)
        => new(cardType, value, suit);
    public static Card GenerateExtraCard(ExtraCards cardType, int value, Suit suit)
        => new(cardType, value, suit);

    public override string ToString()
        => GetName();
    private string GetSymbol()
    {
        if (IsFaceDown)
            return PokerConstants.CARDBACKEMOJI;
        return _emoji ?? _value.ToString();
    }
    private string GetName()
    {
        if (IsFaceDown)
            return $"Face down card.";
        var ofSuit = (byte)_suit > 0 ? $" of {_suit}" : "";
        return $"{_name}{ofSuit}";
    }
}
