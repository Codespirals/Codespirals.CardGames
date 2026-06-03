using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codespirals.CardGames.Poker.BlackJack;

/// <inheritdoc/>
public interface IBlackJackPlayer<TCard, TDeck> : IPlayer<TCard>, ICanPlayRounds, ICanBet, ICanBust, IHasPoints
    where TCard : IPokerCard
{
    /// <summary>
    /// The value of the player's hand
    /// </summary>
    public int HandValue { get; }
    /// <summary>
    /// The player stands on their current hand
    /// </summary>
    public void Stand();
}
