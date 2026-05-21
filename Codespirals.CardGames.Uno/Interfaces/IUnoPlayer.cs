using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames.Uno;

public interface IUnoPlayer<TCard> : IPlayer<TCard>, ICanPlayRounds
    where TCard : IUnoCard
{
}
