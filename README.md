Harry Potter TCG (Unity)
========================

A rewrite of my forever-project in the Unity game engine, initially written in Flash Actionscript 3.0.

Introduction
------------
HPTCG is a recreation of the old Harry Potter Trading Card Game developed (and sadly discontinued) by Wizards of the Coast.
The initial focus of this project was a single player experience through AI opponents and random deck generation for replayability, the goal has since shifted to implementing a multiplayer player interface and matchmaking for players to connect to each other from anywhere. This was made possible by the switch to the Unity3D game engine and the Photon Cloud service.

Once the main game is implemented the plan is to create a tutorial scene and introduces players to the game and allows them to try out many deck combinations before hopping into the matchmaking service.

Planned Features
----------------
* Multiplayer Matchmaking, pits two players with randomly generated decks against each other.
* A basic tutorial system for introducing a new player to the game, this will have separate mechanics to walk the player through how the card game works and will probably include a pre-built deck to ease the player into the concept.
* A progress system to track the player's achievements and wins/losses, maybe an achievement system to track the player's various accomplishments in each game.

To Test the Project
-------------------
Testing the project in its current state will be rather futile as most of the functionality is currently in the process of being ported to the multiplayer client, however, you can download the source and open the project up in Unity to see how it is structured, if you are interested.
You will need to download the Unity3D Game Engine [here](http://unity3d.com/). Open the scene located in **/Assets/Scenes/MultiplayerLobby.unity** and hit the play button at the top of the unity window. The game will automatically connect to the Photon Server and wait for another player to join, you can open up multiple instances of the game by creating a build (ctrl-B) and running it alongside the unity editor. Once two clients are connected the decks will appear and automatically shuffle, each player will draw 7 cards and the first turn will begin.

As of this writing (2015-6-3) the clients are not ye synchronized and any moves you do will only affect your own client, synchronizing the RPC calls between player actions is the next big step that I am undertaking.
