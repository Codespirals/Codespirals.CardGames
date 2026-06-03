using System.Collections.ObjectModel;

namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc cref="IBlackJackPlayer{TCard, TDeck}"/>
public class BlackJackPlayer : Player<PokerCard>, IBlackJackPlayer<PokerCard, PokerDeck>
{
    internal readonly List<PokerCard> _hand = [];
    internal bool _isOutForRound;
    internal int _maxScore = 21;

    /// <summary>
    /// How much cash this player still has to bet with
    /// </summary>
    public int TotalPoints { get; private set; } = 1;
    /// <inheritdoc/>
    public int CurrentBet { get; private set; }
    /// <inheritdoc/>
    public bool IsOutForRound => _isOutForRound || IsBusted || TappedOut;
    /// <inheritdoc/>
    public bool IsBusted => HandValue > _maxScore;
    /// <inheritdoc/>
    public bool TappedOut => TotalPoints <= 0 && CurrentBet <= 0;
    /// <inheritdoc/>
    public int HandValue => CalculateHandValue();

    internal BlackJackPlayer(string name, int startingCash, int maxScore = 21) : base(name)
    {
        _maxScore = maxScore;
        TotalPoints = startingCash;
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
        _hand.Clear();
        CurrentBet = 0;
        _isOutForRound = false;
    }

    private int CalculateHandValue()
    {
        var value = _hand.Sum(c => c.Value);
        foreach (var ace in _hand.Where(c => c.Value == 11))
        {
            if (value <= _maxScore)
                break;
            value -= 10;
        }
        if (value > _maxScore)
        {
            Bust();
        }
        return value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Name} | Cash: {TotalPoints} | Bet: {CurrentBet}";
}
