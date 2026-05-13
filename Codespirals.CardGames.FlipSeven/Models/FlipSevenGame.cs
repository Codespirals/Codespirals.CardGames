using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenGame : IFlipSevenGame<FlipSevenGame, FlipSevenPlayer, FlipSevenDeck, FlipSevenCard>
{
    private readonly List<FlipSevenPlayer> _players = [];
    private FlipSevenPlayer _currentPlayer;
    private int _currentRound = 0;

    public FlipSevenDeck Deck { get; } = FlipSevenDeckBuilder.CreateStandardDeck();
    public ReadOnlyCollection<FlipSevenPlayer> Players => _players.AsReadOnly();
    public FlipSevenPlayer CurrentPlayer => _currentPlayer;
    public int CurrentRound => _currentRound;
    public int WinningScore { get; set; } = 200;
    public int NumbersToFlip { get; set; } = 7;
    public int FlipNumberBonus { get; set; } = 15;
    public bool RoundActive => !_players.All(p => p.IsOutForRound);
    public bool GameOver => !RoundActive && _players.Any(p => p.TotalPoints > WinningScore);

    private FlipSevenGame(int players, int numbersToFlip = 7, int flipNumberBonus = 15, int winningScore = 200, FlipSevenDeck? deck = null)
    {
        for (var i = 0; i < players; i++)
        {
            _players.Add(FlipSevenPlayer.GeneratePlayer(this, i));
        }
        Deck = deck ?? FlipSevenDeckBuilder.CreateStandardDeck();
        NumbersToFlip = numbersToFlip;
        WinningScore = winningScore;
        _currentPlayer = _players.First();
        Deck.Shuffle();
    }

    public static FlipSevenGame SetUp(int players)
        => new(players);
    public static FlipSevenGame SetUp(int players, int numbersToFlip = 7, int flipNumberBonus = 15, int winningScore = 200, FlipSevenDeck? deck = null)
        => new(players, numbersToFlip, flipNumberBonus, winningScore, deck);

    public FlipSevenPlayer GetCurrentPlayer() => _currentPlayer;

    public void StartRound()
    {
        _currentRound++;
        foreach (var player in _players)
        {
            player.Reactivate();
        }
        _currentPlayer = Players[_currentRound % _players.Count];
    }

    public int? Bank(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;
        return player.Bank();
    }

    public FlipSevenCard? Flip(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;
        var card = Deck.Draw();
        if (card is null)
            return null;

        // action cards need to be played immediately
        if (card.CardType is CardType.Flip or CardType.Freeze or CardType.SecondChance)
            return card;

        player.AddCardToHand(card);

        // is number and player already has number
        if (card.CardType == CardType.Number && player.Hand.Any(c => c.CardType == CardType.Number && c.Value == card.Value))
        {
            var hasSecondChance = player.Hand.FirstOrDefault(c => c.CardType == CardType.SecondChance);
            if (hasSecondChance is not null)
            {
                player.Discard(card);
                player.Discard(hasSecondChance);
            }
            else
                player.DeactivateForRound();
        }
        if (player.NumberCardsInHand == NumbersToFlip)
        {
            EndRound();
        }

        return card;
    }

    /// <summary>
    /// Attempt to give a card to another player.
    /// This can only be a card of type <see cref="CardType.Flip"/>, <see cref="CardType.Freeze"/> or <see cref="CardType.SecondChance"/>.
    /// However a player can only have 1 <see cref="CardType.SecondChance"/> at a time.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <returns>
    /// <list type="bullet">
    /// <item>An list of cards containing the cards the player flipped</item>
    /// <item>An empty list if the transfer was successful but didn't result in any drawn cards</item>
    /// <item><see langword="null"/> if the transfer failed</item>
    /// </list> 
    /// </returns>
    public IEnumerable<FlipSevenCard>? TryGivePlayerCard(FlipSevenPlayer player, FlipSevenCard card)
    {
        if (player.IsOutForRound)
            return null;
        switch (card.CardType)
        {
            case CardType.Number or CardType.Multiplier or CardType.BonusAdd:
                return null;
            case CardType.SecondChance:
                if (player.Hand.Any(c => c.CardType == CardType.SecondChance))
                    return null;
                player.AddCardToHand(card);
                return [];
            case CardType.Flip:
                return Flip(player, card.Value);
            case CardType.Freeze:
                player.Freeze();
                return [];
            default:
                return null;
        }
    }

    public IEnumerable<FlipSevenCard> Flip(FlipSevenPlayer player, int number)
    {
        for (int i = 0; i < number; i++)
        {
            var card = Flip(player);
            if (card is null)
                yield break;
            yield return card;
        }
    }

    public int? Freeze(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;
        return player.Freeze();
    }

    public void MoveToNextPlayer()
    {
        if (_players.All(p => p.IsOutForRound) || GameOver)
        {
            EndRound();
            return;
        }

        var nextPlayerIndex = (_players.IndexOf(CurrentPlayer) + 1) % _players.Count;
        _currentPlayer = _players[nextPlayerIndex];

        if (_currentPlayer.IsOutForRound)
            MoveToNextPlayer();
    }

    public void EndRound()
    {
        foreach (var player in _players.Where(p => !p.IsOutForRound))
        {
            player.Bank();
        }
    }

    public (FlipSevenPlayer Player, int Winnings)[] CalculateCurrentPotentialPointGain()
    {
        (FlipSevenPlayer Player, int WinningMultiplier)[] results = [];
        foreach (var player in Players)
        {
            var winnings = -1;
            if (player.State == PlayerStates.Busted)
            {
                winnings = 0;
            }
            else
            {
                winnings = player.HandPoints;
            }
            results = results.Append((player, winnings)).ToArray();
        }
        return results;
    }

    public void PayOut()
    {
        if (Players.Any(p => !p.IsOutForRound))
        {
            return;
        }
        foreach (var item in CalculateCurrentPotentialPointGain())
        {
            item.Player.AddWinnings(item.Winnings);
        }
    }

    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.TotalPoints);
    }
}
