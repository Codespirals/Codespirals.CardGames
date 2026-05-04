using Codespirals.CardGames.Poker.BlackJack;
using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker;
public class BlackJackPlayer : IBlackJackPlayer<Deck, Card>
{
    internal readonly BlackJackGame _game;
    internal readonly List<Card> _hand = [];
    internal bool _isOutForRound;

    public string Name { get; set; }
    public int Cash { get; private set; } = 1;
    public int CurrentBet { get; private set; }
    public bool IsOutForRound => _isOutForRound || TappedOut;
    public bool TappedOut => Cash <= 0 && CurrentBet <= 0;
    public int HandValue => CalculateHandValue();
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();

    private BlackJackPlayer(BlackJackGame game, string name, int startingCash)
    {
        _game = game;
        Name = name;
        Cash = startingCash;
    }
    private BlackJackPlayer(BlackJackGame game, int number, int startingCash) : this(game, $"Player {number + 1}", startingCash)
    {

    }
    public static BlackJackPlayer GeneratePlayer(BlackJackGame game, string name, int startingCash)
        => new BlackJackPlayer(game, name, startingCash);
    public static BlackJackPlayer GeneratePlayer(BlackJackGame game, int number, int startingCash)
        => new BlackJackPlayer(game, number, startingCash);
    public void AddCardToHand(Card card) => _hand.Add(card);
    public void Discard(Card card)
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
        CurrentBet = Math.Clamp(amount, 0, Cash);
        Cash -= CurrentBet;
    }

    public void DoubleDown(Card card)
    {
        if (Cash >= CurrentBet)
        {
            Bet(CurrentBet);
        }
        AddCardToHand(card);
        Stand();
    }

    public void Stand()
        => _isOutForRound = true;

    public void Bust()
    {
        CurrentBet = 0;
        _isOutForRound = true;
    }

    public void AddWinnings(int multiplier = 1)
    {
        Cash += CurrentBet * multiplier;
        CurrentBet = 0;
    }

    public void DeactivateForRound()
    {
        DiscardAll();
        CurrentBet = 0;
        _isOutForRound = true;
    }

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
            if (value <= _game.WinningScore)
                break;
            value -= 10;
        }
        if (value > _game.WinningScore)
        {
            Bust();
        }
        return value;
    }

    public override string ToString()
        => $"{Name} {(Hand.Count != 0 ? "Hand: " : "")}{string.Join('|', Hand.OrderByDescending(c => c.IsFaceDown).Select(c => c.Name))} Hand total: {HandValue} Cash:{Cash} Current Bet:{CurrentBet}";
}
