namespace Codespirals.CardGames.Poker.BlackJack;
public interface IBlackJackGame<TSelf, TPlayer, TDeck, TCard> : IGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer, TDeck, TCard>, IHasBets
    where TSelf : IBlackJackGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IBlackJackPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : IPokerCard
{
    /// <summary>
    /// The dealer of the game
    /// </summary>
    public TPlayer Dealer { get; }
    /// <summary>
    /// What the winning score is
    /// </summary>
    /// <remarks>For Blackjack the default should be 21</remarks>
    int BlackJackScore { get; }
    /// <summary>
    /// How many cards every player draws automatically at the beginning of the round
    /// </summary>
    int DrawAtStartOfRound { get; }
    /// <summary>
    /// Set up a new game with extended parameters
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="players"></param>
    /// <param name="minBet"></param>
    /// <param name="winningScore"></param>
    /// <param name="startingCash"></param>
    /// <param name="automaticallyIncreaseStakeAfterRound"></param>
    /// <param name="drawAtStartOfRound"></param>
    /// <returns></returns>
    static abstract TSelf SetUp(int players, int minBet, int winningScore, int startingCash, int automaticallyIncreaseStakeAfterRound, int drawAtStartOfRound, TDeck? deck);
    /// <summary>
    /// Make the player draw a card
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    TCard? Hit(TPlayer player);
    /// <summary>
    /// Make the player double down on their bet
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    TCard? DoubleDown(TPlayer player);
    /// <summary>
    /// Make the player stick to their current hand
    /// </summary>
    /// <param name="player"></param>
    void Stand(TPlayer player);
    /// <summary>
    /// Make the dealer play his round.
    /// </summary>
    /// <remarks>The dealer counts cards. The house has an advantage.</remarks>
    public TCard? PlayDealer();
    /// <summary>
    /// End the round and return information about the winnings of the round
    /// </summary>
    /// <returns></returns>
    (BlackJackPlayer Player, int WinningMultiplier)[] CalculateWinningsOfRound();
}
