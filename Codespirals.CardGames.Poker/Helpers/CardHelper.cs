namespace Codespirals.CardGames.Poker;
public static class CardHelper
{
    public static Card NoCard()
        => new(0b_0000_0000);
    public static Card Ace(Suit suit)
        => FromSuitAndValue(suit, 0b_0000_0001);
    public static Card NumberCard(Suit suit, byte value)
        => value is > 1 and < 11 ? FromSuitAndValue(suit, value) : NoCard();
    public static Card Jack(Suit suit)
        => FromSuitAndValue(suit, 0b_0000_1011);
    public static Card Queen(Suit suit)
        => FromSuitAndValue(suit, 0b_0000_1100);
    public static Card King(Suit suit)
        => FromSuitAndValue(suit, 0b_0000_1101);
    public static Card Joker()
        => new(0b_0000_1110);
    public static Card Fool()
        => new(0b_0000_1111);

    private static Card FromSuitAndValue(Suit suit, byte value)
        => new((byte)(((int)suit << 4) | value));
}
