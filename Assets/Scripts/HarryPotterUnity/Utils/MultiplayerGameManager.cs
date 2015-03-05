using System;
using System.Collections;
using HarryPotterUnity.Game;
using HarryPotterUnity.UI;
using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Utils
{
    public class MultiplayerGameManager : MonoBehaviour
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        private MultiplayerLobbyHudManager _multiplayerLobbyHudManager;

        public void Start()
        {
            _multiplayerLobbyHudManager = GameObject.Find("MultiplayerLobbyHudManager").GetComponent<MultiplayerLobbyHudManager>();

            if(!_multiplayerLobbyHudManager) throw new Exception("MultiplayerGameManager could not find MultiplayerLobbyHudManager!");
        }

        private void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            Player1 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
            Player2 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
        }

        [RPC]
        public void StartGameRpc(int rngSeed)
        {
            _multiplayerLobbyHudManager.DisableHud();

            Random.seed = rngSeed;
            SpawnPlayers();
            StartGame();
        }

        private void StartGame()
        {
            if(!Player1 || !Player2) throw new Exception("Error: One of the players was not properly instantiated!");

            Player1.OppositePlayer = Player2;
            Player2.OppositePlayer = Player1;

            Player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            Player1.InitDeck();
            Player2.InitDeck();

            StartCoroutine(_beginGameSequence());
        }

        private IEnumerator _beginGameSequence()
        {
            Player1.Deck.Shuffle();
            Player2.Deck.Shuffle();
            yield return new WaitForSeconds(2.4f);
            Player1.DrawInitialHand();
            Player2.DrawInitialHand();

            Player1.InitTurn();
        }

        public void OnDestroy()
        {
            if(Player1) Destroy(Player1.gameObject);
            if(Player2) Destroy(Player2.gameObject);
        }
    }
}
