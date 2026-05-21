using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.Uno;

public interface IUnoGame<TSelf, TPlayer, TDeck, TCard> : ICardGame<TSelf, TDeck, TCard>, IRoundBased, IHasPlayers<TPlayer>, IHasActivityLog<LogEntry>, IHasPrompt
    where TSelf : IUnoGame<TSelf, TPlayer, TDeck, TCard>
    where TPlayer : IUnoPlayer<TCard>
    where TDeck : IDeck<TCard>
    where TCard : IUnoCard
{

}
