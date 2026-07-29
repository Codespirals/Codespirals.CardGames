# Codespirals.CardGames.FlipNumber

An implementation of a card game very simiar to - but legally distinct from Flip Number in C#

## Contents

### Deck Builder

An easy to use, static deck builder to make your own custom FlipNumber decks. Simply call with

    var builder = FlipNumber.DeckBuilder.Begin()

Add all the things you want to your deck and finish by

    var customDeck = builder.Build()

### Main Game Object

To start a game simply call

    var game = FlipNumber.Game.SetUp(numberOfPlayers);

if you want to use a custom deck, add the deck as a parameter in the SetUp method.

    var game = FlipNumber.Game.SetUp(numberOfPlayers);

And then follow the game progression from there.