Harry Potter TCG (Unity)
========================

A rewrite of my forever-project in the Unity game engine, initially written in Flash Actionscript 3.0.

Introduction
------------
HPTCG is a recreation of the old Harry Potter Trading Card Game developed (and sadly discontinued) by Wizards of the Coast.
The initial focus of this project was a single player experience through AI opponents and random deck generation for replayability, the goal has since shifted to implementing a multiplayer client with random matchmaking for players to connect to each other from anywhere. This was made possible by the switch to the Unity3D game engine and the Photon Cloud service.

Once the main game is implemented the plan is to create a tutorial scene that introduces players to the game and allows them to try out many deck combinations before hopping into the matchmaking service.

Planned Features
----------------
* Multiplayer Matchmaking, pits two players with randomly generated decks against each other.
* A basic tutorial system for introducing a new player to the game, this will have separate mechanics to walk the player through how the card game works and will probably include a pre-built deck to ease the player into the concept.
* A progress system to track the player's achievements and wins/losses, maybe an achievement system to track the player's various accomplishments in each game.

To Test the Project
-------------------
You can find a build of the project in the releases tab, read there for more details on what  the current public version entails.

I don't expect there to be a ton of people using this, as it is quite a niche project, but you can now download the game and hop into the matchmaking service without needing to compile it yourself!

If you have no one to play with, you can test the game against yourself, to do so, you can open up multiple instances of the game and hit Find Match at the same time. Once the two clients are connected the decks will appear and automatically shuffle, each player will draw 7 cards and the first turn will begin.

Basic functionality has been implemented and you should be able to complete a game against another player, you will probably notice that there aren't that many cards implemented, I have spent the past few months working on the infrastructure that will allow more complicated cards to be thrown into the random card pool, you can expect the card pool to grow substantially in the next few months as the more complicated features get built into the engine.

Thanks for reading and enjoy the game! Feel free to drop me a message if you enjoy it.
