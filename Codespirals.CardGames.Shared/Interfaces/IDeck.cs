using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IDeck<TCard>
    where TCard : ICard
{
    public ReadOnlyCollection<TCard> StartingCards { get; }
    public ReadOnlyCollection<TCard> CardPool { get; }
    public ReadOnlyCollection<TCard> DiscardPile { get; }

    public TCard Draw();
    public IEnumerable<TCard> Draw(int numberOfCards);
    public IEnumerable<TCard> Peek(int numberOfCards);
    public void PutOnDiscardPile(TCard card);
    public void ShuffleDiscardPileIntoDeck();
    public void Shuffle();
    public void Order();
    public void Reset();
}
