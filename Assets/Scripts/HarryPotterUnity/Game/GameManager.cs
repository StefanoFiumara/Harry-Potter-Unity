using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public class GameManager : Photon.MonoBehaviour
    {
        //TODO: Are these references kept between clients?
        public Player Player1;
        public Player Player2;

        //reference this instead of the two instances above?
        public List<Player> Players;

        public void Start()
        {
            Players = new List<Player>();
        }


        public void StartGame()
        {
            if(Players.Count != 2) throw new Exception(string.Format("There are {0} players in the room, there must be exactly 2!", Players.Count));
            
            Player1 = Players[0];
            Player2 = Players[1];

            Player1.OppositePlayer = Player2;
            Player2.OppositePlayer = Player1;

            Player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);


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

        public void DestroyPlayers()
        {
            Destroy(Player1);
            Destroy(Player2);
        }
    }
}
