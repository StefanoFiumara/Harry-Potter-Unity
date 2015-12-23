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
        private Player _player1;
        private Player _player2;

        private MenuManager _menuManager;
        private List<BaseMenu> _allMenuScreens;

        private const string LOBBY_VERSION = "v0.2-dev";

        private static readonly TypedLobby DefaultLobby = new TypedLobby(LOBBY_VERSION, LobbyType.Default);

        public void Start()
        {
            Log.Init("HP-TCG", "Harry Potter TCG Log");
            Log.Write("Initialize Log File");
            
            _menuManager = FindObjectOfType<MenuManager>();
            _allMenuScreens = FindObjectsOfType<BaseMenu>().ToList();

            GameManager.Network = photonView;

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
            PhotonNetwork.JoinLobby( DefaultLobby );
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
                Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);

                Camera.main.transform.rotation = rotation;
                GameManager.PreviewCamera.transform.rotation = rotation;
            }
        }

        [UsedImplicitly]
        public void OnPhotonPlayerConnected()
        {
            int seed = Random.Range(int.MinValue, int.MaxValue);

            Log.Write("New Player has Connected, Starting Game...");
            photonView.RPC("StartGameRpc", PhotonTargets.All, seed);
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

            DestroyPlayerObjects();

            _menuManager.ShowMenu(_allMenuScreens.First(m => m.name.Contains("MainMenuContainer")));

            GameManager.TweenQueue.Reset();
        }

        [PunRPC, UsedImplicitly]
        public void StartGameRpc(int rngSeed)
        {
            //Synchronize the Random Number Generator for both clients with the given seed
            Random.seed = rngSeed;

            SpawnPlayers();
            SetPlayerProperties();
            SetUpGameplayHud();
            InitPlayerDecks();
            BeginGame();
        }
        
        private void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            _player1 = ( (GameObject) Instantiate(playerObject) ).GetComponent<Player>();
            _player2 = ( (GameObject) Instantiate(playerObject) ).GetComponent<Player>();

            if (!_player1 || !_player2)
            {
                Log.Error("One of the players was not properly instantiated, Report this error!");
            }
        }

        private void SetPlayerProperties()
        {
            _player1.IsLocalPlayer = PhotonNetwork.player.isMasterClient;
            _player2.IsLocalPlayer = !_player1.IsLocalPlayer;

            _player1.OppositePlayer = _player2;
            _player2.OppositePlayer = _player1;

            _player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            _player1.NetworkId = 0;
            _player2.NetworkId = 1;
        }

        private void SetUpGameplayHud()
        {
            var gameplayMenu = _allMenuScreens.First(m => m.name.Contains("GameplayMenuContainer")) as GameplayMenu;

            if (gameplayMenu != null)
            {
                gameplayMenu.LocalPlayer = _player1.IsLocalPlayer ? _player1 : _player2;
                gameplayMenu.RemotePlayer = _player1.IsLocalPlayer ? _player2 : _player1;

                _menuManager.ShowMenu(gameplayMenu);
            }
            else
            {
                Log.Error("SetUpGameplayHud() Failed, could not find GameplayMenuContainer, Report this error!");
            }
        }
        

        #region Legacy UI Code, Probably belongs somewhere else
        /*
        private void SetPlayer2Local()
        {
            _player1.TurnIndicator = _hudManager.TurnIndicatorRemote;
            _player2.TurnIndicator = _hudManager.TurnIndicatorLocal;

            _player1.ActionsLeftLabel = _hudManager.ActionsLeftRemote;
            _player2.ActionsLeftLabel = _hudManager.ActionsLeftLocal;

            _player1.CardsLeftLabel = _hudManager.CardsLeftRemote;
            _player2.CardsLeftLabel = _hudManager.CardsLeftLocal;
        }

        private void SetPlayer1Local()
        {
            _player1.TurnIndicator = _hudManager.TurnIndicatorLocal;
            _player2.TurnIndicator = _hudManager.TurnIndicatorRemote;

            _player1.ActionsLeftLabel = _hudManager.ActionsLeftLocal;
            _player2.ActionsLeftLabel = _hudManager.ActionsLeftRemote;

            _player1.CardsLeftLabel = _hudManager.CardsLeftLocal;
            _player2.CardsLeftLabel = _hudManager.CardsLeftRemote;
        }
        */
        #endregion
            

        private void InitPlayerDecks()
        {
            int p1Id = PhotonNetwork.isMasterClient ? 0 : 1;
            int p2Id = p1Id == 0 ? 1 : 0;

            var p1LessonsBytes = PhotonNetwork.playerList[p1Id].customProperties["lessons"] as byte[];
            var p2LessonsBytes = PhotonNetwork.playerList[p2Id].customProperties["lessons"] as byte[];

            if (p1LessonsBytes == null || p2LessonsBytes == null)
            {
                Log.Error("p1 or p2 selected lessons are null!");
                return;
            }

            var p1SelectedLessons = p1LessonsBytes.Select(n => (LessonTypes) n).ToList();
            var p2SelectedLessons = p2LessonsBytes.Select(n => (LessonTypes) n).ToList();

            GameManager._networkIdCounter = 0;
            GameManager.AllCards.Clear();

            DeckGenerator.ResetStartingCharacterPool();

            Log.Write("Generating Player Decks");
            _player1.InitDeck(p1SelectedLessons);
            _player2.InitDeck(p2SelectedLessons);
        }

        private void BeginGame()
        {
            Log.Write("Game setup complete, starting match");
            _player1.Deck.SpawnStartingCharacter();
            _player2.Deck.SpawnStartingCharacter();

            _player1.Deck.Shuffle();
            _player2.Deck.Shuffle();

            _player1.DrawInitialHand();
            _player2.DrawInitialHand();

            _player1.BeginTurn();
        }

        private void DestroyPlayerObjects()
        {
            Destroy(_player1.gameObject);
            Destroy(_player2.gameObject);
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
            card.MouseUpAction();
            card.Player.OnCardPlayed(card);
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
            persistentCard.OnSelectedAction();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteDrawActionOnPlayer(byte id)
        {
            var player = id == 0 ? _player1 : _player2;

            Log.Write("Player {0} Draws a Card", player.NetworkId + 1);
            player.Deck.DrawCard();
            player.UseActions();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteInputCardById(byte id, params byte[] selectedCardIds)
        {
            var card = GameManager.AllCards.Find(c => c.NetworkId == id);
            
            var selectedCards = selectedCardIds.Select(cardId => GameManager.AllCards.Find(c => c.NetworkId == cardId)).ToList();
            
            Log.Write("Player {0} plays card {1} targeting {2}", 
                card.Player.NetworkId + 1, 
                card.CardName, 
                string.Join(",", selectedCards.Select(c => c.CardName).ToArray()));

            card.MouseUpAction(selectedCards);
            card.Player.OnCardPlayed(card, selectedCards);

            card.Player.EnableAllCards();
            card.Player.ClearHighlightComponent();

            card.Player.OppositePlayer.EnableAllCards();
            card.Player.OppositePlayer.ClearHighlightComponent();
        }

        [PunRPC, UsedImplicitly]
        public void ExecuteSkipAction()
        {
            if (_player1.CanUseActions())
            {
                Log.Write("Player 1 skipped an action");
                _player1.UseActions();
            }
            else if (_player2.CanUseActions())
            {
                Log.Write("Player 2 skipped an action");
                _player2.UseActions();
            }
            else
            {
                Log.Error("ExecuteSkipAction() failed to identify which player wants to skip their Action!");
            }
        }
    }
}
