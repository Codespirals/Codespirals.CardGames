
using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Player : IOpenHandedPlayer<Card>
{
    private List<Card> _hand = [];
    private int _points = 0;
    public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    public int HandCount => _hand.Count(c => c.CardType == CardType.Number);
    public int Points => _points;

    public Player()
    {

    }

    public void Draw(Card card)
    {
        _hand.Add(card);
    }
    public void Bank()
    {
        var roundPoints = 0;
        foreach (var card in _hand.OrderBy(c => c.CardType))
        {
            switch (card.CardType)
            {
                case CardType.Number:
                    roundPoints += card.Value;
                    break;
                case CardType.TimesTwo:
                    roundPoints *= 2;
                    break;
                case CardType.BonusAdd:
                    roundPoints += card.Value;
                    break;
                default:
                    break;
            }
        }
        _points += roundPoints;
    }
}
