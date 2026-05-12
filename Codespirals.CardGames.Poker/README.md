# Codespirals.CardGames.Poker

A set of Poker-based card games

## Currently implemented:

### Deck Builder

An easy to use, static deck builder to make your own custom Poker decks. Simply call with

    var builder = PokerDeckBuilder.Begin()

Add all the things you want to your deck and finish by

    builder.Build()

### Blackjack

##### Main Game Object

Simply call

    var game = BlackJackGame.SetUp([numberOfpLayers])

and start playing!