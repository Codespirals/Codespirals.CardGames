using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;

/// <inheritdoc />
public class FlipSevenGame : IFlipSevenGame<FlipSevenGame, FlipSevenPlayer, FlipSevenDeck, FlipSevenCard>
{
    private readonly List<FlipSevenPlayer> _players = [];
    private FlipSevenPlayer _currentPlayer;
    private int _currentRound = 0;

    /// <inheritdoc />
    public FlipSevenDeck Deck { get; } = FlipSevenDeckBuilder.CreateStandardDeck();
    /// <inheritdoc />
    public ReadOnlyCollection<FlipSevenPlayer> Players => _players.AsReadOnly();
    /// <inheritdoc />
    public FlipSevenPlayer CurrentPlayer => _currentPlayer;
    /// <inheritdoc />
    public int CurrentRound => _currentRound;
    /// <inheritdoc />
    public int WinningScore { get; set; } = 200;
    /// <inheritdoc />
    public int NumbersToFlip { get; set; } = 7;
    /// <inheritdoc />
    public int FlipNumberBonus { get; set; } = 15;
    /// <inheritdoc />
    public bool RoundActive => !_players.All(p => p.IsOutForRound);
    /// <inheritdoc />
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

    /// <inheritdoc />
    public static FlipSevenGame SetUp(int players)
        => new(players);
    /// <inheritdoc />
    public static FlipSevenGame SetUp(int players, int numbersToFlip = 7, int flipNumberBonus = 15, int winningScore = 200, FlipSevenDeck? deck = null)
        => new(players, numbersToFlip, flipNumberBonus, winningScore, deck);

    /// <inheritdoc />
    public FlipSevenPlayer GetCurrentPlayer() => _currentPlayer;

    /// <inheritdoc />
    public void StartRound()
    {
        _currentRound++;
        foreach (var player in _players)
        {
            player.Reactivate();
        }
        _currentPlayer = Players[_currentRound % _players.Count];
    }
    /// <inheritdoc />
    public int? Bank(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;
        return player.Bank();
    }
    /// <inheritdoc />
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
    /// <inheritdoc />
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
    /// <inheritdoc />
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
    /// <inheritdoc />
    public int? Freeze(FlipSevenPlayer player)
    {
        if (player.IsOutForRound)
            return null;
        return player.Freeze();
    }
    /// <inheritdoc />
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
    /// <inheritdoc />
    public void EndRound()
    {
        foreach (var player in _players.Where(p => !p.IsOutForRound))
        {
            player.Bank();
        }
    }
    /// <inheritdoc />
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
    /// <inheritdoc />
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
    /// <inheritdoc />
    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.TotalPoints);
    }
}
