using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
/// <summary>
/// A game of Flip Seven
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TPlayer"></typeparam>
/// <typeparam name="TDeck"></typeparam>
/// <typeparam name="TCard"></typeparam>
public interface IFlipSevenGame<TSelf, TPlayer, TDeck, TCard> : ICardGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer>, IPlayersHavePoints<TPlayer>, IHasPrompt, IHasActivityLog<LogEntry>
    where TSelf : IFlipSevenGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IFlipSevenPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : IFlipSevenCard
{
    /// <summary>
    /// Set up for a new game
    /// </summary>
    /// <param name="players"></param>
    /// <param name="numbersToFlip"></param>
    /// <param name="flipNumberBonus"></param>
    /// <param name="winningScore"></param>
    /// <param name="deck"></param>
    /// <returns></returns>
    abstract static TSelf SetUp(int players, int numbersToFlip = 7, int flipNumberBonus = 15, int winningScore = 200, FlipSevenDeck? deck = null);
    /// <summary>
    /// The score needed to win the game. Default should be 200
    /// </summary>
    int WinningScore { get; }
    /// <summary>
    /// How many numbers are needed to end a round and gain bonus points. Default should be 7
    /// </summary>
    int NumbersToFlip { get; }
    /// <summary>
    /// How many bonus points you get for getting <see cref="NumbersToFlip"/>. Default should be 15
    /// </summary>
    int FlipNumberBonus { get; }
    /// <summary>
    /// Dictates wether a player can have more than one second chance
    /// </summary>
    bool PlayersCanHaveMultipleSecondChances { get; }
    /// <summary>
    /// Active action cards get added to this queue to be used
    /// </summary>
    ReadOnlyCollection<(FlipSevenPlayer Player, FlipSevenCard ActionCard)> ActionCardQueue { get; } 
    /// <summary>
    /// Remove the current player from the round and bank the points
    /// </summary>
    /// <returns></returns>
    void Bank();
    /// <summary>
    /// Flip a card for the current player
    /// </summary>
    /// <returns></returns>
    TCard? Flip();
    /// <summary>
    /// Get the players you can target with <see cref="UseActionCard(TPlayer, TCard)"/>
    /// </summary>
    /// <param name="card">The card you're targeting with.</param>
    /// <returns></returns>
    IEnumerable<TPlayer>? GetValidTargets(TCard card);
    /// <summary>
    /// Attempt to give a card to another player.
    /// This can only be a card of type <see cref="CardType.Flip"/>, <see cref="CardType.Freeze"/> or <see cref="CardType.SecondChance"/>.
    /// However a player can only have 1 <see cref="CardType.SecondChance"/> at a time.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <returns>
    /// <list type="bullet">
    /// <item>An list of cards containing the cards the player flipped</item>
    /// <item>An empty list if the transfer was successful but didn't result in any drawn cards</item>
    /// <item><see langword="null"/> if the transfer failed</item>
    /// </list> 
    /// </returns>
    IEnumerable<TCard>? UseActionCard(TPlayer player, TCard card);
    /// <summary>
    /// Flip multiple cards for a the player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    IEnumerable<TCard> ForceFlip(TPlayer player, int number);
    /// <summary>
    /// Freeze a player and force them to bank
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    int? Freeze(TPlayer player);
    /// <summary>
    /// Get the winner of the round. Returns <see langword="null"/> if no winner has been decided yet
    /// </summary>
    /// <returns></returns>
    TPlayer? GetWinner();
}
