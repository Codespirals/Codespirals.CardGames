using System;
using System.Collections.Generic;
using System.Text;

namespace Codespirals.CardGames;

/// <summary>
/// A deck builder to create <typeparamref name="TDeck"/>s
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TDeck"></typeparam>
/// <typeparam name="TCard"></typeparam>
public interface IDeckBuilder<TSelf, TDeck, TCard>
    where TSelf : IDeckBuilder<TSelf, TDeck, TCard>
    where TDeck : IDeck<TCard>
    where TCard : ICard
{
    /// <summary>
    /// Begin building a <typeparamref name="TDeck"/>
    /// </summary>
    /// <returns></returns>
    abstract static TSelf Begin();
    /// <summary>
    /// Finish building this <typeparamref name="TDeck"/>
    /// </summary>
    /// <returns></returns>
    public TDeck Build();
    /// <summary>
    /// Create a standard <typeparamref name="TDeck"/>
    /// </summary>
    /// <returns></returns>
    abstract static TDeck CreateStandardDeck();
}
