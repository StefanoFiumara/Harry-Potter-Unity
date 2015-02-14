using System.Collections;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public class GameManager : Photon.MonoBehaviour
    {
        //TODO: Are these references kept between clients?
        public Player Player1;
        public Player Player2;
        
        public void StartGame()
        {
            SpawnPlayer1();
            SpawnPlayer2();
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

        private void SpawnPlayer1()
        {
            // Player1 = ((GameObject)Instantiate(PlayerObject)).GetComponent<Player>();
            Player1 = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0).GetComponent<Player>();
            Player1.transform.parent = transform;

            Player1.OppositePlayer = Player2;
        }
        private void SpawnPlayer2()
        {
           // Player2 = ((GameObject)Instantiate(PlayerObject)).GetComponent<Player>();
            Player2 = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.Euler(0f, 0f, 180f), 0).GetComponent<Player>();
            Player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
            Player2.transform.parent = transform;

            Player2.OppositePlayer = Player1;
        }

        public void DestroyPlayers()
        {
            Destroy(Player1);
            Destroy(Player2);
        }
    }
}