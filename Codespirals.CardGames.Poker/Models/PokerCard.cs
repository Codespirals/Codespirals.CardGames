namespace Codespirals.CardGames.Poker;

public class PokerCard : IPokerCard
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

    internal PokerCard(string name, int value, Suit suit)
    {
        _suit = suit;
        _name = name;
        _value = value;

        // if the card is of traditinal suits, there's an emoji for it
        if (_suit is >= Suit.Spades and <= Suit.Clubs)
            _emoji = char.ConvertFromUtf32(Convert.ToInt32("01F0A0", 16) + (((int)_suit - 1) * 16) + _value);
    }

    internal PokerCard(NamedCard cardType, int value, Suit suit)
    {
        _suit = suit;
        _name = cardType.ToString();
        _value = value;

        // if the card is of traditinal suits, there's an emoji for it
        if (_suit is >= Suit.Spades and <= Suit.Clubs)
            _emoji = char.ConvertFromUtf32(Convert.ToInt32("01F0A0", 16) + (((int)_suit - 1) * 16) + (int)cardType);
    }

    internal PokerCard(ExtraCard cardType, int value, Suit suit = Suit.Unknown)
    {
        _suit = suit;
        _value = value;
        // joker
        if (cardType is ExtraCard.Joker)
        {
            _name = ExtraCard.Joker.ToString();
            _emoji = PokerConstants.JOKEREMOJI;
        }
        // fool
        else if (cardType is ExtraCard.Fool)
        {
            _name = ExtraCard.Fool.ToString();
            _emoji = PokerConstants.FOOLEMOJI;
        }
        else
        {
            _name = "";
        }
    }

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
