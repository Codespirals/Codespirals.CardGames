using Codespirals.CardGames.Poker.BlackJack;
using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker;
public class BlackJackPlayer : IBlackJackPlayer<PokerDeck, PokerCard>
{
    internal readonly BlackJackGame _game;
    internal readonly List<PokerCard> _hand = [];
    internal bool _isOutForRound;

    public string Name { get; set; }
    public int TotalPoints { get; private set; } = 1;
    public int CurrentBet { get; private set; }
    public bool IsOutForRound => _isOutForRound || IsBusted || TappedOut;
    public bool IsBusted => HandValue > _game.BlackJackScore;
    public bool TappedOut => TotalPoints <= 0 && CurrentBet <= 0;
    public int HandValue => CalculateHandValue();
    public ReadOnlyCollection<PokerCard> Hand => _hand.AsReadOnly();

    private BlackJackPlayer(BlackJackGame game, string name, int startingCash)
    {
        _game = game;
        Name = name;
        TotalPoints = startingCash;
    }
    private BlackJackPlayer(BlackJackGame game, int number, int startingCash) : this(game, $"Player {number + 1}", startingCash)
    {

    }
    public static BlackJackPlayer GeneratePlayer(BlackJackGame game, string name, int startingCash)
        => new BlackJackPlayer(game, name, startingCash);
    public static BlackJackPlayer GeneratePlayer(BlackJackGame game, int number, int startingCash)
        => new BlackJackPlayer(game, number, startingCash);
    public void AddCardToHand(PokerCard card) => _hand.Add(card);
    public void Discard(PokerCard card)
    {
        _hand.Remove(card);
        _game.Deck.PutOnDiscardPile(card);
    }

    public void DiscardAll()
    {
        foreach (var card in _hand)
            _game.Deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    public void Bet(int amount)
    {
        CurrentBet = Math.Clamp(amount, 0, TotalPoints);
        TotalPoints -= CurrentBet;
    }

    public void DoubleDown(PokerCard card)
    {
        Bet(Math.Clamp(CurrentBet, 0, TotalPoints));
        AddCardToHand(card);
        Stand();
    }

    public void Stand()
        => DeactivateForRound();

    public void Bust()
    {
        CurrentBet = 0;
        _isOutForRound = true;
    }

    public void AddWinnings(int winnings = 0)
    {
        TotalPoints += winnings;
        CurrentBet = 0;
    }

    public void DeactivateForRound()
        => _isOutForRound = true;

    public void Reactivate()
    {
        DiscardAll();
        CurrentBet = 0;
        _isOutForRound = false;
    }

    private int CalculateHandValue()
    {
        var value = _hand.Sum(c => c.Value);
        foreach (var ace in _hand.Where(c => c.Value == 11))
        {
            if (value <= _game.BlackJackScore)
                break;
            value -= 10;
        }
        if (value > _game.BlackJackScore)
        {
            Bust();
        }
        return value;
    }

    public override string ToString()
        => $"{Name} {Hand.Select(c => c.Emoji)}";
}
