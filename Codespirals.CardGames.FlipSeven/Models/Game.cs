using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Game : IGame<Game, Deck, Card, Player>
{
    private readonly List<Player> _players = [];
    private int _currentPlayerId = 0;
    private int _currentRound = 0;
    private bool _roundActive;

    public Deck Deck { get; } = new();
    public ReadOnlyCollection<Player> Players => _players.AsReadOnly();
    public int WinningScore { get; set; } = 200;
    public int NumbersToFlip { get; set; } = 7;
    public int RoundsPlayed => _currentRound;
    public int CurrentRound => _currentRound + 1;
    public bool RoundActive => _roundActive;
    public bool GameOver => !_roundActive && _players.Any(p => p.Points > WinningScore);

    private Game(int players, int numberCards = 12, int freezes = 4, int flipThrees = 4, int secondChances = 4, int timesTwos = 1, int bonusCards = 5)
    {
        Deck = new Deck(numberCards, freezes, flipThrees, secondChances, timesTwos, bonusCards);
        NumbersToFlip = (int)Math.Ceiling(numberCards / 2d) + 1;
        Deck.Shuffle();
        for (var i = 0; i < players; i++)
        {
            _players.Add(new Player(i, Deck, NumbersToFlip));
        }
    }

    public static Game SetUp(int players)
        => SetUp(players, 12, 4, 4, 4, 1, 5);
    public static Game SetUp(int players, int numberCards, int freezes, int flipThrees, int secondChances, int timesTwos, int bonusCards)
        => new(players, numberCards, freezes, flipThrees, secondChances, timesTwos, bonusCards);

    public void StartRound()
    {
        _roundActive = true;
        _currentPlayerId = _currentRound % _players.Count;
    }
    public void EndRound()
    {
        foreach (var player in _players)
        {
            if (!player.IsOutForRound)
                player.BankPoints();
            player.State = PlayerStates.Playing;
        }
        _roundActive = false;
    }

    public Player GetCurrentPlayer()
    {
        if (_players.All(p => p.IsOutForRound))
        {
            EndRound();
            return _players[CurrentRound % _players.Count];
        }
        else
        {
            var player = _players[_currentPlayerId];
            while (player is null)
            {
                _currentPlayerId = (_currentPlayerId + 1) % _players.Count;
                player = _players[_currentPlayerId];
            }
            return player;
        }
    }
    public Player MoveToNextPlayer()
    {
        var player = GetCurrentPlayer();
        if (player.HandCount >= NumbersToFlip)
        {
            EndRound();
        }
        _currentPlayerId = (_currentPlayerId + 1) % _players.Count;
        return GetCurrentPlayer();
    }

    public Player? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.Points);
    }

    public static void Freeze(Player player) => player.Freeze();
    public static IEnumerable<Card> FlipThree(Player player)
    {
        List<Card> cards = [];
        for (var i = 0; i < 3; i++)
        {
            cards.Add(player.Draw());
            if (player.IsOutForRound)
                return cards;
        }
        return cards;
    }
}
