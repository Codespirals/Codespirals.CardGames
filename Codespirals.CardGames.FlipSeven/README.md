# Codespirals.CardGames.FlipSeven

An implementation of the card game Flip Seven in C#

## Contents

### Deck Builder

An easy to use, static deck builder to make your own custom Flip7 decks. Simply call with

    var builder = FlipSevenDeckBuilder.Begin()

Add all the things you want to your deck and finish by

    builder.Build()

### Main Game Object

To start a game simply call

    var game = FlipSevenGame.SetUp(playersNumber);

And then follow the game progression from there.