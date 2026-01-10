namespace Codespirals.CardGames;
public static class CardExtensions
{
    public static IEnumerable<TCard> Shuffle<TCard>(this IEnumerable<TCard> list)
        where TCard : ICard
    {
        var n = list.Count();
        var temp = list.ToList();
        while (n > 1)
        {
            n--;
            var k = Random.Shared.Next(n + 1);
            (temp[n], temp[k]) = (temp[k], temp[n]);
        }
        return temp;
    }
}
