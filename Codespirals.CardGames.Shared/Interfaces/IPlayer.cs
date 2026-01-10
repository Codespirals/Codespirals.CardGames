using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IPlayer<TCard>
    where TCard : ICard
{
    public int HandCount { get; }

    public void Draw(TCard card);
}

public interface IOpenHandedPlayer<TCard> : IPlayer<TCard>
    where TCard : ICard
{
    public ReadOnlyCollection<TCard> Hand { get; }
}