using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IDeck<TCard>
    where TCard : ICard
{
    ReadOnlyCollection<TCard> StartingCards { get; }
    ReadOnlyCollection<TCard> CardPool { get; }
    ReadOnlyCollection<TCard> DiscardPile { get; }

    TCard Draw();
    IEnumerable<TCard> Draw(int numberOfCards);
    IEnumerable<TCard> Peek(int numberOfCards);
    void PutOnDiscardPile(TCard card);
    void ShuffleDiscardPileIntoDeck();
    void Shuffle();
    void Order();
    void Reset();
}
