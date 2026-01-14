using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public ReadOnlyCollection<TCard> Hand { get; }
    public string Name { get; set; }
    public int HandCount { get; }

    public void Draw(TCard card);
    public void Discard(TCard card);
    public void DiscardAll();
}