namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenDeckBuilder
{
    private Deck _deck;
    private FlipSevenDeckBuilder()
    {
        _deck = new Deck();
    }
    public static FlipSevenDeckBuilder Begin() => new();
    public FlipSevenDeckBuilder WithNumberCards(int highestNumber = 12)
    {
        _deck.AddStartingCard(new Card(CardType.Number, 0));
        for (var v = highestNumber; v > 0; v--)
        {
            for (var i = 0; i < v; i++)
            {
                _deck.AddStartingCard(new Card(CardType.Number, v));
            }
        }
        return this;
    }
    public FlipSevenDeckBuilder WithBonusCards(int number = 5, int step = 2)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(new Card(CardType.BonusAdd, i * step));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithMultipliers(int number = 1, int multiplier = 2)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(new Card(CardType.Multiplier, multiplier));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithFlipCards(int number = 4, int flip = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(new Card(CardType.Flip, flip));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithFreezes(int number = 4)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(new Card(CardType.Freeze, 0));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithSecondChances(int number = 5)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(new Card(CardType.SecondChance, 0));
        }
        return this;
    }
    public Deck Build()
    {
        _deck.OrderStartingCards();
        _deck.Reset();
        return _deck;
    }
    public static Deck CreateStandardDeck()
        => Begin()
        .WithNumberCards()
        .WithBonusCards()
        .WithFlipCards()
        .WithMultipliers()
        .WithFreezes()
        .WithSecondChances()
        .Build();
}
