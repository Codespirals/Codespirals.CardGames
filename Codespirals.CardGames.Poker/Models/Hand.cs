using System.Collections.ObjectModel;

namespace Codespirals.Poker;
public class Hand
{
    private List<Card> _cards = [];
    public ReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    public void Add(Card card)
        => _cards.Add(card);
    public void Add(IEnumerable<Card> card)
        => _cards.AddRange(card);

    public Card? Take(byte cardCode)
        => _cards.FirstOrDefault(c => c.CardCode == cardCode);
    public IEnumerable<Card> Take(byte[] cardCodes)
        => cardCodes.Select(code => _cards.FirstOrDefault(c => c.CardCode == code)).WhereNotNull();
}
