using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Game;
using HarryPotterUnity.UI;
using JetBrains.Annotations;
using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Utils
{
    [UsedImplicitly]
    public class MultiplayerGameManager : MonoBehaviour
    {
        private Player _player1;
        private Player _player2;

        private MultiplayerLobbyHudManager _multiplayerLobbyHudManager;

        [UsedImplicitly]
        public void Start()
        {
            _multiplayerLobbyHudManager = GameObject.Find("MultiplayerLobbyHudManager").GetComponent<MultiplayerLobbyHudManager>();

            if(!_multiplayerLobbyHudManager) throw new Exception("MultiplayerGameManager could not find MultiplayerLobbyHudManager!");
        }

        [RPC, UsedImplicitly]
        public void StartGameRpc(int rngSeed)
        {
            _multiplayerLobbyHudManager.DisableMainMenuHud();
            _multiplayerLobbyHudManager.EnableGameplayHud();

            Random.seed = rngSeed;
            SpawnPlayers();
            StartGame();
        }

        private void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            _player1 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
            _player2 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();

            if (!_player1 || !_player2) throw new Exception("Error: One of the players was not properly instantiated!");

            _player1.IsLocalPlayer = PhotonNetwork.player.isMasterClient;
            _player2.IsLocalPlayer = !_player1.IsLocalPlayer;

            _player1.OppositePlayer = _player2;
            _player2.OppositePlayer = _player1;

            _player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            if (_player1.IsLocalPlayer)
            {
                _player1.TurnIndicator = _multiplayerLobbyHudManager.TurnIndicatorLocal;
                _player2.TurnIndicator = _multiplayerLobbyHudManager.TurnIndicatorRemote;

                _player1.ActionsLeftLabel = _multiplayerLobbyHudManager.ActionsLeftLocal;
                _player2.ActionsLeftLabel = _multiplayerLobbyHudManager.ActionsLeftRemote;

                _player1.CardsLeftLabel = _multiplayerLobbyHudManager.CardsLeftLocal;
                _player2.CardsLeftLabel = _multiplayerLobbyHudManager.CardsLeftRemote;
            }
            else
            {
                _player1.TurnIndicator = _multiplayerLobbyHudManager.TurnIndicatorRemote;
                _player2.TurnIndicator = _multiplayerLobbyHudManager.TurnIndicatorLocal;

                _player1.ActionsLeftLabel = _multiplayerLobbyHudManager.ActionsLeftRemote;
                _player2.ActionsLeftLabel = _multiplayerLobbyHudManager.ActionsLeftLocal;

                _player1.CardsLeftLabel = _multiplayerLobbyHudManager.CardsLeftRemote;
                _player2.CardsLeftLabel = _multiplayerLobbyHudManager.CardsLeftLocal;
            }
        }

        private void StartGame()
        {
            InitPlayerDecks();

            _player1.MpGameManager = this;
            _player2.MpGameManager = this;

            _player1.NetworkId = 0;
            _player2.NetworkId = 1;

            StartCoroutine(_beginGameSequence());
        }

        private void InitPlayerDecks()
        {
            var p1Id = PhotonNetwork.isMasterClient ? 0 : 1;
            var p2Id = p1Id == 0 ? 1 : 0;

            var p1LessonsBytes = PhotonNetwork.playerList[p1Id].customProperties["lessons"] as byte[];
            var p2LessonsBytes = PhotonNetwork.playerList[p2Id].customProperties["lessons"] as byte[];

            if (p1LessonsBytes == null || p2LessonsBytes == null)
            {
                throw new Exception("p1 or p2 lessons are null!");
            }

            var p1SelectedLessons = Array.ConvertAll(p1LessonsBytes, input => (Lesson.LessonTypes) input).ToList();
            var p2SelectedLessons = Array.ConvertAll(p2LessonsBytes, input => (Lesson.LessonTypes) input).ToList();
            
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

            _player1.InitTurn();
        }

        [UsedImplicitly]
        public void OnDestroy()
        {
            if(_player1) Destroy(_player1.gameObject);
            if(_player2) Destroy(_player2.gameObject);
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
        public void ExecuteInputSpellById(byte id, params byte[] cardIds)
        {
            var card = (GenericSpell) UtilManager.AllCards.Find(c => c.NetworkId == id);
            
            var selectedCards = cardIds.Select(cardId => UtilManager.AllCards.Find(c => c.NetworkId == cardId)).ToList();

            card.AfterInputAction(selectedCards);

            card.Player.EnableAllCards();
            card.Player.OppositePlayer.EnableAllCards();

            card.Player.UseActions(card.ActionCost);
        }
    }
}
