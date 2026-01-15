using System.Collections.ObjectModel;

namespace Codespirals.CardGames;

public interface IPlayer
{
    string Name { get; set; }
    int HandCount { get; }
}
public interface IPlayer<TDeck, TCard> : IPlayer
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    ReadOnlyCollection<TCard> Hand { get; }

    void AddCardToHand(TCard card);
    void Discard(TCard card);
    void DiscardAll();
    void Bust();
}