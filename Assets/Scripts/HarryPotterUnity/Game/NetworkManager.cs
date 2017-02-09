using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using HarryPotterUnity.UI;
using HarryPotterUnity.UI.Menu;
using JetBrains.Annotations;
using UnityLogWrapper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Game
{
    public class NetworkManager : Photon.MonoBehaviour
    {
        private Player Player1 { get; set; }
        private Player Player2 { get; set; }

        private MenuManager MenuManager { get; set; }
        private List<BaseMenu> MenuScreens { get; set; }

        private const string LOBBY_VERSION = "v0.2-dev";

        private static readonly TypedLobby _defaultLobby = new TypedLobby(LOBBY_VERSION, LobbyType.Default);

        public void Awake()
        {
            Log.Write("Initialize Log");

            this.MenuManager = FindObjectOfType<MenuManager>();
            this.MenuScreens = FindObjectsOfType<BaseMenu>().ToList();

            GameManager.Network = this.photonView;

            PhotonNetwork.ConnectUsingSettings(LOBBY_VERSION);
        }

        [UsedImplicitly]
        public void OnConnectedToMaster()
        {
            Log.Write("Connected to Photon Master Server");
            ConnectToPhotonLobby();
        }

        public static void ConnectToPhotonLobby()
        {
            PhotonNetwork.JoinLobby(_defaultLobby );
        }

        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            Log.Write("Joined {0} Lobby", LOBBY_VERSION);
        }

        [UsedImplicitly]
        public void OnJoinedRoom()
        {
            Log.Write("Joined Photon Room, Waiting for Players...");

            if (PhotonNetwork.room.playerCount == 2)
            {
                var p2Rotation = Quaternion.Euler(0f, 0f, 180f);

                Camera.main.transform.rotation = p2Rotation;
                Camera.main.transform.localPosition = new Vector3
                {
                    x = Camera.main.transform.localPosition.x,
                    y = 132f,
                    z = Camera.main.transform.localPosition.z
                };

                GameManager.PreviewCamera.transform.rotation = p2Rotation;
            }
            else
            {
                Camera.main.transform.rotation = Quaternion.identity;
                GameManager.PreviewCamera.transform.rotation = Quaternion.identity;
            }
        }

        [UsedImplicitly]
        public void OnPhotonPlayerConnected()
        {
            int seed = Random.Range(int.MinValue, int.MaxValue);

            Log.Write("New Player has Connected, Starting Game...");
            this.photonView.RPC("StartGameRpc", PhotonTargets.All, seed);
        }
        
        [UsedImplicitly]
        public void OnPhotonRandomJoinFailed()
        {
            string roomName = "Room " + PhotonNetwork.GetRoomList().Length;
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { maxPlayers = 2 }, null);
        }

        [UsedImplicitly]
        public void OnPhotonPlayerDisconnected()
        {
            Log.Write("Opponent Disconnected, Back to Main Menu...");
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        [UsedImplicitly]
        public void OnLeftRoom()
        {
            Log.Write("Player Disconnected, Back to Main Menu");
            
            GameManager.TweenQueue.Reset();
            this.DestroyPlayerObjects();

            this.MenuManager.ShowMenu(this.MenuScreens.First(m => m.name.Contains("MainMenuContainer")));
        }

        private void DestroyPlayerObjects()
        {
            if (this.Player1 != null) Destroy(this.Player1.gameObject);
            if (this.Player2 != null) Destroy(this.Player2.gameObject);
        }

        [PunRPC, UsedImplicitly]
        public void StartGameRpc(int rngSeed)
        {
            //Synchronize the Random Number Generator for both clients with the given seed
            Random.InitState(rngSeed);

            this.SpawnPlayers();
            this.SetPlayerProperties();
            this.SetUpGameplayHud();
            this.InitPlayerDecks();
            this.BeginGame();
        }
        
        private void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            this.Player1 = ( (GameObject) Instantiate(playerObject) ).GetComponent<Player>();
            this.Player2 = ( (GameObject) Instantiate(playerObject) ).GetComponent<Player>();

            if (!this.Player1 || !this.Player2)
            {
                Log.Error("One of the players was not properly instantiated, Report this error!");
            }
        }

        private void SetPlayerProperties()
        {
            this.Player1.IsLocalPlayer = PhotonNetwork.player.isMasterClient;
            this.Player2.IsLocalPlayer = !this.Player1.IsLocalPlayer;

            this.Player1.OppositePlayer = this.Player2;
            this.Player2.OppositePlayer = this.Player1;

            this.Player1.transform.localRotation = Quaternion.identity;
            this.Player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            this.Player1.NetworkId = 0;
            this.Player2.NetworkId = 1;
        }

        private void SetUpGameplayHud()
        {
            var gameplayMenu = this.MenuScreens.FirstOrDefault(m => m.name.Contains("GameplayMenuContainer")) as GameplayMenu;

            if (gameplayMenu != null)
            {
                gameplayMenu.LocalPlayer  = this.Player1.IsLocalPlayer ? this.Player1 : this.Player2;
                gameplayMenu.RemotePlayer = this.Player1.IsLocalPlayer ? this.Player2 : this.Player1;

                this.MenuManager.ShowMenu(gameplayMenu);
            }
            else
            {
                Log.Error("SetUpGameplayHud() Failed, could not find GameplayMenuContainer, Report this error!");
            }
        }
            
        private void InitPlayerDecks()
        {
            int p1Id = PhotonNetwork.isMasterClient ? 1 : 0;
            int p2Id = p1Id == 0 ? 1 : 0;

            var p1LessonsBytes = PhotonNetwork.playerList[p1Id].customProperties["lessons"] as byte[];
            var p2LessonsBytes = PhotonNetwork.playerList[p2Id].customProperties["lessons"] as byte[];

            if (p1LessonsBytes == null || p2LessonsBytes == null)
            {
                Log.Error("p1 or p2 selected lessons are null, report this error!");
                return;
            }

            var p1SelectedLessons = p1LessonsBytes.Select(n => (LessonTypes) n).ToList();
            var p2SelectedLessons = p2LessonsBytes.Select(n => (LessonTypes) n).ToList();

            GameManager.NetworkIdCounter = 0;
            GameManager.AllCards.Clear();

            DeckGenerator.ResetStartingCharacterPool();

            Log.Write("Generating Player Decks");
            this.Player1.InitDeck(p1SelectedLessons);
            this.Player2.InitDeck(p2SelectedLessons);
        }

        private void BeginGame()
        {
            Log.Write("Game setup complete, starting match");
            this.Player1.Deck.SpawnStartingCharacter();
            this.Player2.Deck.SpawnStartingCharacter();

            //Shuffle after drawing the initial hand if debug mode is enabled
            if (GameManager.DebugModeEnabled == false)
            {
                this.Player1.Deck.Shuffle();
                this.Player2.Deck.Shuffle();
                this.Player1.DrawInitialHand();
                this.Player2.DrawInitialHand();
            }
            else
            {
                this.Player1.DrawInitialHand();
                this.Player2.DrawInitialHand();
                this.Player1.Deck.Shuffle();
                this.Player2.Deck.Shuffle();
            }

            this.Player1.BeginTurn();
        }
        
        [PunRPC, UsedImplicitly]
        public void ExecutePlayActionById(byte id)
        {
            var card = GameManager.AllCards.Find(c => c.NetworkId == id);

            if (card == null)
            {   
                Log.Error("ExecutePlayActionById could not find card with Id: " + id);
                return;
            }

            Log.Write("Player {0} Plays {1} from hand", card.Player.NetworkId + 1, card.CardName);
            card.Player.InvokeCardPlayedEvent(card);
            card.PlayFromHand();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteInPlayActionById(byte id)
        {
            BaseCard card = GameManager.AllCards.Find(c => c.NetworkId == id);

            if (card == null)
            {
                Log.Error("ExecuteInPlayActionById could not find card with Id: " + id);
                return;
            }

            var persistentCard = card as IPersistentCard;
            if (persistentCard == null)
            {
                Log.Error("ExecuteInPlayActionById did not receive a PersistentCard!");
                return;
            }
            
            Log.Write("Player {0} Activates {1}'s effect", card.Player.NetworkId + 1, card.CardName);
            persistentCard.OnInPlayAction();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteDrawActionOnPlayer(byte id)
        {
            var player = id == 0 ? this.Player1 : this.Player2;

            Log.Write("Player {0} Draws a Card", player.NetworkId + 1);
            player.Deck.DrawCard();
            player.UseActions();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteInputCardById(byte id, params byte[] selectedCardIds)
        {
            var card = GameManager.AllCards.Find(c => c.NetworkId == id);
            
            var targetedCards = selectedCardIds.Select(cardId => GameManager.AllCards.Find(c => c.NetworkId == cardId)).ToList();
            
            Log.Write("Player {0} plays card {1} targeting {2}", 
                card.Player.NetworkId + 1, 
                card.CardName, 
                string.Join(",", targetedCards.Select(c => c.CardName).ToArray()));


            card.Player.InvokeCardPlayedEvent(card, targetedCards);

            card.PlayFromHand(targetedCards);

            card.Player.EnableAllCards();
            card.Player.ClearHighlightComponent();

            card.Player.OppositePlayer.EnableAllCards();
            card.Player.OppositePlayer.ClearHighlightComponent();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteInPlayInputCardById(byte id, params byte[] selectedCardIds)
        {
            var card = GameManager.AllCards.Find(c => c.NetworkId == id);

            var selectedCards = selectedCardIds.Select(cardId => GameManager.AllCards.Find(c => c.NetworkId == cardId)).ToList();

            Log.Write("Player {0} activates card {1} targeting {2}",
                card.Player.NetworkId + 1,
                card.CardName,
                string.Join(",", selectedCards.Select(c => c.CardName).ToArray()));

            var persistentCard = card as IPersistentCard;
            if (persistentCard == null)
            {
                Log.Error("ExecuteInPlayInputCardById did not receive a PersistentCard!");
                return;
            }

            persistentCard.OnInPlayAction(selectedCards);

            card.Player.EnableAllCards();
            card.Player.ClearHighlightComponent();

            card.Player.OppositePlayer.EnableAllCards();
            card.Player.OppositePlayer.ClearHighlightComponent();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteSkipAction()
        {
            if (this.Player1.CanUseActions())
            {
                Log.Write("Player 1 skipped an action");
                this.Player1.UseActions();
            }
            else if (this.Player2.CanUseActions())
            {
                Log.Write("Player 2 skipped an action");
                this.Player2.UseActions();
            }
            else
            {
                Log.Error("ExecuteSkipAction() failed to identify which player wants to skip their Action!");
            }
        }

        private void OnApplicationQuit()
        {
            Log.SaveToFile("HP-TCG", "Harry Potter TCG Log");
        }
    }
}
