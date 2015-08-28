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
This is a Unity3D Project, to test the game yourself before an official client is out will requiresome rather large downloads, sorry for the inconvenience!

You will need to download the Unity3D Game Engine [here](http://unity3d.com/) (This game has been upgraded to Unity 5.0, so make sure you grab that version). Open the scene located in **/Assets/Scenes/MultiplayerLobby.unity** and hit the play button at the top of the unity window. The game will automatically connect to the Photon Server, select a combination of lessons to use in your deck and click the 'Find Duel' button, which will throw you into the random matchmaking service.

To test the game, you can open up multiple instances of the game by creating a build (ctrl-B) and running it alongside the unity editor. Once two clients are connected the decks will appear and automatically shuffle, each player will draw 7 cards and the first turn will begin.

Basic functionality has been implemented and you should be able to complete a game against another player, you will probably notice that there aren't that many cards implemented, I have spent the past few months doing the multiplayer conversion and haven't been implementing new cards until all the basic functionality is in, you can expect the card pool to grow massively in the next few months once all the multiplayer kinks get worked out.
