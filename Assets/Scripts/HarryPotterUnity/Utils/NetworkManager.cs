using System;
using System.Collections;
using System.Linq;
using HarryPotterUnity.Game;
using HarryPotterUnity.UI;
using JetBrains.Annotations;
using UnityEngine;

using Lessontypes = HarryPotterUnity.Cards.Generic.Lesson.LessonTypes;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Utils
{
    [UsedImplicitly]
    public class NetworkManager : Photon.MonoBehaviour
    {
        private Player _player1;
        private Player _player2;

        private HudManager _hudManager;

        [UsedImplicitly]
        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");

            _hudManager = FindObjectOfType<HudManager>();

            if (!_hudManager)
            {
                Debug.LogError("Network Manager could not find Hud Manager in Scene!");
            }
        }
        
        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            _hudManager.InitMainMenu();
        }

        [UsedImplicitly]
        public void OnJoinedRoom()
        {
            if (PhotonNetwork.room.playerCount == 1) return;

            _hudManager.SetPlayer2CameraRotation();
        }

        [UsedImplicitly]
        public void OnPhotonPlayerConnected()
        {
            int seed = Random.Range(int.MinValue, int.MaxValue);

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
            _hudManager.BackToMainMenu();
        }

        [RPC, UsedImplicitly]
        public void StartGameRpc(int rngSeed)
        {
            _hudManager.DisableMainMenuHud();
            _hudManager.EnableGameplayHud();

            Random.seed = rngSeed;
            SpawnPlayers();
            StartGame();
        }

        private void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            _player1 = ( (GameObject) Instantiate(playerObject) ).GetComponent<Player>();
            _player2 = ( (GameObject) Instantiate(playerObject) ).GetComponent<Player>();

            if (!_player1 || !_player2)
            {
                Debug.LogError("One of the players was not properly instantiated!");
                return;
            }

            SetPlayerProperties();
        }

        private void SetPlayerProperties()
        {
            _player1.IsLocalPlayer = PhotonNetwork.player.isMasterClient;
            _player2.IsLocalPlayer = !_player1.IsLocalPlayer;

            _player1.OppositePlayer = _player2;
            _player2.OppositePlayer = _player1;

            _player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            _player1.EndGamePanel = _hudManager.EndGamePanel;
            _player2.EndGamePanel = _hudManager.EndGamePanel;

            if (_player1.IsLocalPlayer)
            {
                SetPlayer1Local();
            }
            else
            {
                SetPlayer2Local();
            }
        }

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

        private void StartGame()
        {
            InitPlayerDecks();

            _player1.NetworkManager = this;
            _player2.NetworkManager = this;

            _player1.NetworkId = 0;
            _player2.NetworkId = 1;

            StartCoroutine(_beginGameSequence());
        }

        private void InitPlayerDecks()
        {
            int p1Id = PhotonNetwork.isMasterClient ? 0 : 1;
            int p2Id = p1Id == 0 ? 1 : 0;

            var p1LessonsBytes = PhotonNetwork.playerList[p1Id].customProperties["lessons"] as byte[];
            var p2LessonsBytes = PhotonNetwork.playerList[p2Id].customProperties["lessons"] as byte[];

            if (p1LessonsBytes == null || p2LessonsBytes == null)
            {
                Debug.LogError("p1 or p2 selected lessons are null!");
                return;
            }

            var p1SelectedLessons = p1LessonsBytes.Select(n => (Lessontypes) n).ToList();
            var p2SelectedLessons = p2LessonsBytes.Select(n => (Lessontypes) n).ToList();

            UtilManager.NetworkIdCounter = 0;
            UtilManager.AllCards.Clear();

            _player1.InitDeck(p1SelectedLessons);
            _player2.InitDeck(p2SelectedLessons);
        }

        private IEnumerator _beginGameSequence()
        {
            _player1.Deck.Shuffle();
            _player2.Deck.Shuffle();
            yield return new WaitForSeconds(2.4f);
            _player1.DrawInitialHand();
            _player2.DrawInitialHand();

            _player1.InitTurn(true);
        }

        public void DestroyPlayerObjects()
        {
            Destroy(_player1.gameObject);
            Destroy(_player2.gameObject);
        }

        [RPC, UsedImplicitly]
        public void ExecutePlayActionById(byte id)
        {
            var card = UtilManager.AllCards.Find(c => c.NetworkId == id);

            if (card == null)
            {
                throw new Exception("ExecutePlayActionById could not find card with Id: " + id);
            }

            card.MouseUpAction();
        }

        [RPC, UsedImplicitly]
        public void ExecuteDrawActionOnPlayer(byte id)
        {
            var player = id == 0 ? _player1 : _player2;

            player.Deck.DrawCard();
            player.UseActions();
        }

        [RPC, UsedImplicitly]
        public void ExecuteInputCardById(byte id, params byte[] selectedCardIds)
        {
            var card = UtilManager.AllCards.Find(c => c.NetworkId == id);
            
            var selectedCards = selectedCardIds.Select(cardId => UtilManager.AllCards.Find(c => c.NetworkId == cardId)).ToList();

            card.MouseUpAction(selectedCards);

            card.Player.EnableAllCards();
            card.Player.OppositePlayer.EnableAllCards();
        }

        [RPC, UsedImplicitly]
        public void ExecuteSkipAction()
        {
            if (_player1.CanUseActions())
            {
                _player1.UseActions();
            }
            else if (_player2.CanUseActions())
            {
                _player2.UseActions();
            }
            else
            {
                Debug.LogError("ExecuteSkipAction() failed to identify which player wants to skip their Action!");
            }
        }

        public static IEnumerator WaitForGameOverMessage(Player sender)
        {
            while (UtilManager.TweenQueue.TweenQueueIsEmpty)
            {
                yield return null;
            }

            if (sender.IsLocalPlayer)
            {
                sender.ShowGameOverLoseMessage();
            }
            else
            {
                sender.ShowGameOverWinMesssage();
            }
        }
    }
}
