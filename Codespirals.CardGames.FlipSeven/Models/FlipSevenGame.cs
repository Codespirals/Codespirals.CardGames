using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class FlipSevenGame : IFlipSevenGame<FlipSevenGame, FlipSevenPlayer, FlipSevenDeck, FlipSevenCard>
{
    private readonly List<FlipSevenPlayer> _players = [];
    private FlipSevenPlayer _currentPlayer;
    private int _currentRound = 0;

    public FlipSevenDeck Deck { get; } = FlipSevenDeckBuilder.CreateStandardDeck();
    public ReadOnlyCollection<FlipSevenPlayer> Players => _players.AsReadOnly();
    public ReadOnlyCollection<FlipSevenPlayer> ActivePlayers => _players.Where(p => !p.IsOutForRound).ToList().AsReadOnly();
    public int WinningScore { get; set; } = 200;
    public int NumbersToFlip { get; set; } = 7;
    public FlipSevenPlayer CurrentPlayer => _currentPlayer;
    public int CurrentRound => _currentRound + 1;
    public bool RoundActive => ActivePlayers.Count > 0;
    public bool GameOver => !RoundActive && _players.Any(p => p.BankedPoints > WinningScore);

    private FlipSevenGame(int players, FlipSevenDeck deck, int numbersToFlip = 7, int winningScore = 200)
    {
        for (var i = 0; i < players; i++)
        {
            _players.Add(FlipSevenPlayer.GeneratePlayer(this, i));
        }
        Deck = deck;
        NumbersToFlip = numbersToFlip;
        WinningScore = winningScore;
        _currentPlayer = _players.First();
        Deck.Shuffle();
    }

    public static FlipSevenDeckBuilder StartBuildingCustomDeck()
        => FlipSevenDeckBuilder.Begin();

    public static FlipSevenGame SetUp(int players)
        => new(players, FlipSevenDeckBuilder.CreateStandardDeck());
    public static FlipSevenGame SetUp(int players, FlipSevenDeck deck, int numbersToFlip = 7, int winningScore = 200)
        => new(players, deck, numbersToFlip, winningScore);

    public FlipSevenPlayer GetCurrentPlayer() => _currentPlayer;

    public void StartRound()
    {
        foreach (var player in _players)
        {
            player.Reactivate();
        }
        var startingPlayer = _currentRound % _players.Count;
        _currentPlayer = Players[startingPlayer];
    }

    public void MoveToNextPlayer()
    {
        if (ActivePlayers.Count == 0)
        {
            EndRound();
            return;
        }
        var nextPlayerIndex = (Players.IndexOf(GetCurrentPlayer()) + 1) % Players.Count;
        _currentPlayer = Players[nextPlayerIndex];
        if (_currentPlayer.IsOutForRound)
            MoveToNextPlayer();
    }

    public FlipSevenCard Flip(FlipSevenPlayer player)
    {
        var card = Deck.Draw();
        player.AddCardToHand(card);
        return card;
    }

    public void Freeze(FlipSevenPlayer player)
        => player.Freeze();

    public void EndRound()
    {
        foreach (var player in _players)
        {
            if (!player.IsOutForRound)
                player.BankPoints();
        }
        _currentRound++;
        _currentPlayer = Players[_currentRound % _players.Count];
    }

    public FlipSevenPlayer? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.BankedPoints);
    }
}
