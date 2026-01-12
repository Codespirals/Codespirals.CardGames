using System.Collections.ObjectModel;

namespace Codespirals.CardGames;
public interface IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public string Name { get; set; }
    public int HandCount { get; }

    public void AddCardToHand(TCard card);
    public void Discard(TCard card, TDeck deck);
    public void DiscardAll(TDeck deck);
}

public interface IPlayerInGameWithOpenHand<TDeck, TCard> : IPlayer<TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    public ReadOnlyCollection<TCard> Hand { get; }
}