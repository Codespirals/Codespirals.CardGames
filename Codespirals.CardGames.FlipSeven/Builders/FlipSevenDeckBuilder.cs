namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenDeckBuilder
{
    private readonly FlipSevenDeck _deck;
    private FlipSevenDeckBuilder()
    {
        _deck = new FlipSevenDeck();
    }
    public static FlipSevenDeckBuilder Begin()
        => new();

    public FlipSevenDeckBuilder RefreshOnEmpty(bool refresh = true)
    {
        _deck.RefreshOnEmpty = true;
        return this;
    }

    public FlipSevenDeckBuilder WithNumberCards(int highestNumber = 12)
    {
        _deck.AddStartingCard(FlipSevenCard.GenerateNumberCard(0));
        for (var v = highestNumber; v > 0; v--)
        {
            for (var i = 0; i < v; i++)
            {
                _deck.AddStartingCard(FlipSevenCard.GenerateNumberCard(v));
            }
        }
        return this;
    }
    public FlipSevenDeckBuilder WithBonusCards(int[] values)
    {
        foreach (var value in values)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateBonusAddCard(value));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithMultipliers(int number = 1, int multiplier = 2)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateMultiplierCard(multiplier));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithFlipCards(int number = 3, int flip = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateFlipCard(flip));
        }
        return this;
    }
    public FlipSevenDeckBuilder WithFreezes(int number = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateFreezeCard());
        }
        return this;
    }
    public FlipSevenDeckBuilder WithSecondChances(int number = 3)
    {
        for (var i = 1; i <= number; i++)
        {
            _deck.AddStartingCard(FlipSevenCard.GenerateSecondChanceCard());
        }
        return this;
    }
    public FlipSevenDeck Build()
    {
        _deck.Reset();
        return _deck;
    }

    public static FlipSevenDeck CreateStandardDeck()
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
