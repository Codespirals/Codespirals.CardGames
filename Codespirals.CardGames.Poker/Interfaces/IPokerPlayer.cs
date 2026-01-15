namespace Codespirals.CardGames.Poker;
public interface IPokerPlayer<TDeck, TCard> : IPlayer<TDeck, TCard>, ICanPlayRounds
    where TDeck : IPokerDeck<TCard>
    where TCard : IPokerCard
{
    int Points { get; }
    int CurrentBet { get; }
    bool TappedOut { get; }

    void Bet(int amount);
    void AddWinnings(int amount);
}
