namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc cref="IBlackJackPlayer{TCard, TDeck}"/>
public class BlackJackPlayer : Player<PokerCard>, IBlackJackPlayer<PokerCard, PokerDeck>
{
    internal readonly List<PokerCard> _hand = [];
    private int _cash = 100;

    /// <inheritdoc/>
    public PlayerState State { get; private set; }
    /// <summary>
    /// How much cash this player still has to bet with
    /// </summary>
    public int TotalPoints 
    {
        get => _cash;
        private set => _cash = Math.Clamp(value, 0, int.MaxValue); 
    }
    /// <inheritdoc/>
    public int CurrentBet { get; private set; }
    /// <inheritdoc/>
    public bool IsOutForRound => State != PlayerState.Playing || TappedOut;
    /// <inheritdoc/>
    public bool IsBusted => State == PlayerState.Busted;
    /// <inheritdoc/>
    public bool TappedOut => TotalPoints <= 0 && CurrentBet <= 0;

    internal BlackJackPlayer(string name, int startingCash) : base(name)
    {
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
        => State = PlayerState.Standing;

    /// <inheritdoc/>
    public void Bust()
    {
        CurrentBet = 0;
        State = PlayerState.Busted;
    }

    /// <inheritdoc/>
    public void DeactivateForRound()
        => Stand();

    /// <inheritdoc/>
    public void AddPoints(int points = 0)
    {
        TotalPoints += points;
        CurrentBet = 0;
    }

    /// <inheritdoc/>
    public void Reactivate()
    {
        CurrentBet = 0;
        State = PlayerState.Playing;
    }

    /// <inheritdoc/>
    public override string ToString() 
        => $"{Name} | Cash: {TotalPoints} | Bet: {CurrentBet}";
}
