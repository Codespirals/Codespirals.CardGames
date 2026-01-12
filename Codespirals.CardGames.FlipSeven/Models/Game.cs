using System.Collections.ObjectModel;

namespace Codespirals.CardGames.FlipSeven;
public class Game : IFlipSevenGame<Game, Player, Deck, Card>
{
    private readonly List<Player> _players = [];
    private Player _currentPlayer;
    private int _currentRound = 0;
    private bool _roundActive;

    public Deck Deck { get; } = new();
    public ReadOnlyCollection<Player> Players => _players.AsReadOnly();
    public ReadOnlyCollection<Player> ActivePlayers => _players.Where(p => !p.IsOutForRound).ToList().AsReadOnly();
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
            _players.Add(new Player(i));
        }
        _currentPlayer = _players.First();
    }

    public static Game SetUp(int players)
        => SetUp(players, 12, 4, 4, 4, 1, 5);
    public static Game SetUp(int players, int numberCards, int freezes, int flipThrees, int secondChances, int timesTwos, int bonusCards)
        => new(players, numberCards, freezes, flipThrees, secondChances, timesTwos, bonusCards);

    public void StartRound()
    {
        _roundActive = true;
        var startingPlayer = _currentRound % _players.Count;
        _currentPlayer = Players[startingPlayer];
    }

    public Player GetCurrentPlayer() => _currentPlayer;

    public void MoveToNextPlayer()
    {
        if (ActivePlayers.Count == 0)
        {
            EndRound();
            var startingPlayer = _currentRound % _players.Count;
            _currentPlayer = Players[startingPlayer];
        }
        var currentPlayerIndex = Players.IndexOf(GetCurrentPlayer());
        for (var i = 1; i <= Players.Count; i++)
        {
            var nextPlayerIndex = (currentPlayerIndex + i) % Players.Count;
            var player = Players[nextPlayerIndex];
            if (player.IsOutForRound)
                continue;
            _currentPlayer = player;
        }
    }

    public Card Flip(Player player)
    {
        var card = Deck.Draw();
        if (card.CardType is CardType.Number or CardType.SecondChance or CardType.BonusAdd or CardType.TimesTwo)
            player.AddCardToHand(card);
        return card;
    }

    public void Freeze(Player player)
        => player.Freeze(Deck);

    public void EndRound()
    {
        foreach (var player in _players)
        {
            if (!player.IsOutForRound)
                player.BankPoints(Deck);
            player.Reactivate(Deck);
        }
        _roundActive = false;
        _currentRound++;
    }

    public Player? GetWinner()
    {
        if (!GameOver)
            return null;
        return _players.MaxBy(p => p.Points);
    }
}
