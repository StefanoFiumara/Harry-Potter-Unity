using System;
using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Game;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public class PlayerPhotonRpcManager : Photon.MonoBehaviour
    {
        public Player Player1;
        public Player Player2;
 
        public void SpawnPlayers()
        {
            var playerObject = Resources.Load("Player");
            Player1 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
            Player2 = ((GameObject)Instantiate(playerObject)).GetComponent<Player>();
        }

        [RPC]
        public void StartGameRpc(int rngSeed)
        {
            Debug.Log("Seed: " + rngSeed);
            UnityEngine.Random.seed = rngSeed;
            SpawnPlayers();
            StartGame();
        }

        public void StartGame()
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
    }
}
