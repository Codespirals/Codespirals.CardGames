namespace Codespirals.CardGames.Poker.BlackJack;
/// <summary>
/// A game of blackjack
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TPlayer"></typeparam>
/// <typeparam name="TDeck"></typeparam>
/// <typeparam name="TCard"></typeparam>
public interface IBlackJackGame<TSelf, TPlayer, TDeck, TCard> : ICardGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer>, IHasBets<TPlayer>, IHasActivityLog<LogEntry>, IHasPrompt
    where TSelf : IBlackJackGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IBlackJackPlayer<TCard>
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
    /// <returns>If the dealer drew a card or null if it's the dealer is already out.</returns>
    public bool? PlayDealer();
}
