using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codespirals.CardGames.Poker.BlackJack;

public interface IBlackJackPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>, ICanPlayRounds, ICanBet, ICanBust
    where TDeck : IPokerDeck<TCard>
    where TCard : IPokerCard
{
    public int HandValue { get; }
    public void Stand();
    public void DoubleDown(TCard card);
}
