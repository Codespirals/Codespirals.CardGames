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
    public bool RoundActive => !_players.All(p => p.IsOutForRound);
    public bool GameOver => !RoundActive && _players.Any(p => p.BankedPoints > WinningScore);

    private FlipSevenGame(int players, int numbersToFlip = 7, int winningScore = 200, FlipSevenDeck? deck = null)
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
    public static FlipSevenGame SetUp(int players, int numbersToFlip = 7, int winningScore = 200, FlipSevenDeck? deck = null)
        => new(players, numbersToFlip, winningScore, deck);

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

    public FlipSevenCard? Flip(FlipSevenPlayer player)
    {
        var card = Deck.Draw();
        if (card is not null)
            player.AddCardToHand(card);
        return card;
    }

    public IEnumerable<FlipSevenCard> Flip(FlipSevenPlayer player, int number)
    {
        for (int i = 0; i < number; i++)
        {
            var card = Deck.Draw();
            if (card is null || player.IsBusted)
            {
                yield break;
            }
            player.AddCardToHand(card);
            yield return card;
        }
    }

    public void Freeze(FlipSevenPlayer player)
        => player.Freeze();

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
        foreach (var player in _players)
        {
            if (!player.IsOutForRound)
                player.BankPoints();
        }
    }

    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.BankedPoints);
    }
}
