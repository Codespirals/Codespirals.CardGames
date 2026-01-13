using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Game : IFlipSevenGame<Game, Player, Deck, Card>
{
    private readonly List<Player> _players = [];
    private Player _currentPlayer;
    private int _currentRound = 0;
    private bool _roundActive;

    public Deck Deck { get; } = FlipSevenDeckBuilder.CreateStandardDeck();
    public ReadOnlyCollection<Player> Players => _players.AsReadOnly();
    public ReadOnlyCollection<Player> ActivePlayers => _players.Where(p => !p.IsOutForRound).ToList().AsReadOnly();
    public int WinningScore { get; set; } = 200;
    public int NumbersToFlip { get; set; } = 7;
    public int RoundsPlayed => _currentRound;
    public int CurrentRound => _currentRound + 1;
    public bool RoundActive => _roundActive;
    public bool GameOver => !_roundActive && _players.Any(p => p.BankedPoints > WinningScore);

    private Game(int players, Deck deck, int numbersToFlip = 7, int winningScore = 200)
    {
        for (var i = 0; i < players; i++)
        {
            _players.Add(new Player(this, i));
        }
        Deck = deck;
        NumbersToFlip = numbersToFlip;
        WinningScore = winningScore;
        _currentPlayer = _players.First();
        Deck.Shuffle();
    }

    public static FlipSevenDeckBuilder StartBuildingCustomDeck()
        => FlipSevenDeckBuilder.Begin();

    public static Game SetUp(int players)
        => new (players, FlipSevenDeckBuilder.CreateStandardDeck());
    public static Game SetUp(int players, Deck deck, int numbersToFlip = 7, int winningScore = 200)
        => new(players, deck, numbersToFlip, winningScore);

    public Player GetCurrentPlayer() => _currentPlayer;

    public void StartRound()
    {
        _roundActive = true;
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

    public Card Flip(Player player)
    {
        var card = Deck.Draw();
        return player.Flip(card);
    }

    public void Freeze(Player player)
        => player.Freeze();

    public void EndRound()
    {
        foreach (var player in _players)
        {
            if (!player.IsOutForRound)
                player.BankPoints();
            player.Reactivate();
        }
        _roundActive = false;
        _currentRound++;
        _currentPlayer = Players[_currentRound % _players.Count];
    }

    public Player? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.BankedPoints);
    }
}
