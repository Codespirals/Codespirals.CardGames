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
    /// Automatically increase the stakes at the end of the round by this much
    /// </summary>
    int IncreaseStakeAfterRound { get; }
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
    /// <param name="dealerCanCountCards"></param>
    /// <returns></returns>
    static abstract TSelf SetUp(int players, int startingCash, int minBet = 1, int winningScore = 21, int automaticallyIncreaseStakeAfterRound = 1, int drawAtStartOfRound = 2, bool dealerCanCountCards = false, TDeck? deck = default);
    /// <summary>
    /// The current player draws a card
    /// </summary>
    /// <returns></returns>
    TCard? Hit();
    /// <summary>
    /// The current player doubles down on their bet
    /// </summary>
    /// <returns></returns>
    TCard? DoubleDown();
    /// <summary>
    /// The current player sticks to their current hand
    /// </summary>
    void Stand();
    /// <summary>
    /// Make the dealer play his round.
    /// </summary>
    /// <remarks>The dealer counts cards. The house has an advantage.</remarks>
    /// <returns>If the dealer drew a card or null if it's the dealer is already out.</returns>
    public bool? PlayDealer();
}
