using System;
using System.Collections;
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

        private void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            _player1 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
            _player2 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
        }

        [RPC, UsedImplicitly]
        public void StartGameRpc(int rngSeed)
        {
            _multiplayerLobbyHudManager.DisableHud();

            Random.seed = rngSeed;
            SpawnPlayers();
            StartGame();
        }

        private void StartGame()
        {
            if(!_player1 || !_player2) throw new Exception("Error: One of the players was not properly instantiated!");

            _player1.OppositePlayer = _player2;
            _player2.OppositePlayer = _player1;

            _player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            _player1.InitDeck();
            _player2.InitDeck();

            StartCoroutine(_beginGameSequence());
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
    }
}
