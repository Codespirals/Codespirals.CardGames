using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Codespirals.CardGames.Uno;

public class UnoPlayer : IUnoPlayer<UnoCard>
{
    public ReadOnlyCollection<UnoCard> Hand => throw new NotImplementedException();

    public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsOutForRound => throw new NotImplementedException();

    public void AddCardToHand(UnoCard card) => throw new NotImplementedException();
    public void DeactivateForRound() => throw new NotImplementedException();
    public void Discard(UnoCard card) => throw new NotImplementedException();
    public void DiscardAll() => throw new NotImplementedException();
    public void Reactivate() => throw new NotImplementedException();
}
