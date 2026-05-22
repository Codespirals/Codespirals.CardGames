namespace Codespirals.CardGames.Poker;
public interface IPokerCard : ICard, ICanBeFaceDown
{
    string? Emoji { get; }
}
