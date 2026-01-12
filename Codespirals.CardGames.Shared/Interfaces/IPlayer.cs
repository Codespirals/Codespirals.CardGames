using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public TDeck Deck { get; }
    public int Id { get; }
    public int HandCount { get; }

    public TCard Draw();
    public void Discard(TCard card);
    public void DiscardAll();
}

public interface IPlayerInGameWithOpenHand<TDeck, TCard> : IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public ReadOnlyCollection<TCard> Hand { get; }
}