// See https://aka.ms/new-console-template for more information
using Codespirals.CardGames;
using Codespirals.CardGames.FlipSeven;
using Codespirals.CardGames.TestingConsole.Games;

Console.WriteLine("Hello, World!");

var game = Game.SetUp(4);
foreach (var card in game.Deck.CardPool)
{
    Console.WriteLine($"{card.Name}");
}
Console.WriteLine($"Total cards: {game.Deck.CardPool.Count}");