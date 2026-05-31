using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc cref="IBlackJackPlayer{TCard}"/>
public class BlackJackPlayer : IBlackJackPlayer<PokerCard>
{
    internal readonly BlackJackGame _game;
    internal readonly List<PokerCard> _hand = [];
    internal bool _isOutForRound;

    internal int Id => _game.Players.IndexOf(this);
    /// <inheritdoc/>
    public string Name { get; set; }
    /// <summary>
    /// How much cash this player still has to bet with
    /// </summary>
    public int TotalPoints { get; private set; } = 1;
    /// <inheritdoc/>
    public int CurrentBet { get; private set; }
    /// <inheritdoc/>
    public bool IsOutForRound => _isOutForRound || IsBusted || TappedOut;
    /// <inheritdoc/>
    public bool IsBusted => HandValue > _game.BlackJackScore;
    /// <inheritdoc/>
    public bool TappedOut => TotalPoints <= 0 && CurrentBet <= 0;
    /// <inheritdoc/>
    public int HandValue => CalculateHandValue();
    /// <inheritdoc/>
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
    /// <inheritdoc/>
    public static BlackJackPlayer GeneratePlayer(BlackJackGame game, string name, int startingCash)
        => new BlackJackPlayer(game, name, startingCash);
    /// <inheritdoc/>
    public static BlackJackPlayer GeneratePlayer(BlackJackGame game, int number, int startingCash)
        => new BlackJackPlayer(game, number, startingCash);
    /// <inheritdoc/>
    public void AddCardToHand(PokerCard card) => _hand.Add(card);
    /// <inheritdoc/>
    public void Discard(PokerCard card)
    {
        _hand.Remove(card);
        _game.Deck.PutOnDiscardPile(card);
    }

    /// <inheritdoc/>
    public void DiscardAll()
    {
        foreach (var card in _hand)
            _game.Deck.PutOnDiscardPile(card);
        _hand.Clear();
    }

    /// <inheritdoc/>
    public void Bet(int amount)
    {
        CurrentBet = Math.Clamp(amount, 0, TotalPoints);
        TotalPoints -= CurrentBet;
    }

    /// <inheritdoc/>
    public void Stand()
        => DeactivateForRound();

    /// <inheritdoc/>
    public void Bust()
    {
        CurrentBet = 0;
        _isOutForRound = true;
    }

    /// <inheritdoc/>
    public void AddPoints(int points = 0)
    {
        TotalPoints += points;
        CurrentBet = 0;
    }

    /// <inheritdoc/>
    public void DeactivateForRound()
        => _isOutForRound = true;
    
    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override string ToString() => $"{Name} Current Bet: {CurrentBet}";
}
