using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// A player in a card game played with <typeparamref name="TCard"/>s
/// </summary>
/// <typeparam name="TCard"></typeparam>
public abstract class Player<TCard> : IPlayer<TCard>
    where TCard : ICard
{
    internal readonly List<TCard> _hand = [];
    internal bool _isOutForRound;

    /// <inheritdoc/>
    public string Name { get; set; }
    /// <inheritdoc/>
    public ReadOnlyCollection<TCard> Hand => _hand.AsReadOnly();
    /// <summary>
    /// A player of an unspecified card game
    /// </summary>
    /// <param name="name"></param>
    public Player(string name)
    {
        Name = name;
    }
    /// <inheritdoc/>
    public void AddCardToHand(TCard card) => _hand.Add(card);
    /// <inheritdoc/>
    public TCard? Discard(TCard card)
    {
        var wasRemoved = _hand.Remove(card);
        if (!wasRemoved)
            return default;
        return card;
    }

    /// <inheritdoc/>
    public IEnumerable<TCard> DiscardAll()
    {
        List<TCard> cards = _hand;
        _hand.Clear();
        return cards;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Name} - {Hand.Count} Cards in hand";
}
