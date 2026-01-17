namespace Codespirals.CardGames.Poker;

public class Card : IPokerCard
{
    private readonly string _emoji = "?";
    private readonly int _value = 0;
    private readonly Suit _suit = Suit.Unknown;
    private readonly string _name = "";
    public int Value => IsFaceDown ? 0 : _value;
    public string Name => GetName();
    public Suit Suit => IsFaceDown ? Suit.Unknown : _suit;
    public string? Emoji => IsFaceDown ? Constants.CARDBACKEMOJI : _emoji;
    public bool IsFaceDown { get; internal set; }

    public Card(string name, int value, Suit suit)
    {
        _suit = suit;
        _name = name;
        _value = value;

        // if the card is of traditinal suits, there's an emoji for it
        if (_suit is >= Suit.Diamonds and <= Suit.Spades)
            _emoji = char.ConvertFromUtf32(Convert.ToInt32("01F0A0", 16) + _value + (((int)_suit - 1) * 16));
    }
    public Card(NamedCards cardType, int value, Suit suit) : this(cardType.ToString(), value, suit)
    {

    }

    public Card(ExtraCards cardType, int value, Suit suit = Suit.Unknown)
    {
        _suit = suit;
        _value = value;
        // joker
        if (cardType is ExtraCards.Joker)
        {
            _name = ExtraCards.Joker.ToString();
            _emoji = Constants.JOKEREMOJI;
        }
        // fool
        else if (cardType is ExtraCards.Fool)
        {
            _name = ExtraCards.Fool.ToString();
            _emoji = Constants.FOOLEMOJI;
        }
        else
        {
            _name = "";
        }
    }

    public override string ToString()
        => GetName();
    private string GetName()
    {
        if (IsFaceDown)
            return $"Face down card.";
        var ofSuit = (byte)_suit > 0 ? $" of {_suit}" : "";
        return $"{_name}{ofSuit}";
    }
}
