namespace Codespirals.CardGames.Poker;

/// <summary>
/// The suits to be used on the cards
/// </summary>
/// <remarks>
/// Since I'm using a nibble to store suits, I have some spots open after <see cref="Unknown"/> and the traditional 4.
/// Because I'm swiss I decide to add the traditional swiss suits (the fourth, <see cref="Hearts"/> already exists).
/// Technically there's room for 8 more
/// </remarks>
public enum Suit
{
    Unknown,
    Spades,
    Hearts,
    Diamonds,
    Clubs,
    Bells,
    Acorns,
    Flowers,
    Shields,
}
