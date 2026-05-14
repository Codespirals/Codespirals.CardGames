using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
/// <summary>
/// This game has players
/// </summary>
/// <typeparam name="TPlayer"></typeparam>
public interface IHasPlayers<TPlayer>
    where TPlayer : IPlayer
{
    /// <summary>
    /// The players involved in this game
    /// </summary>
    ReadOnlyCollection<TPlayer> Players { get; }
    /// <summary>
    /// The currently active player
    /// </summary>
    TPlayer CurrentPlayer { get; }
    /// <summary>
    /// Move to next player
    /// </summary>
    void MoveToNextPlayer();
}
