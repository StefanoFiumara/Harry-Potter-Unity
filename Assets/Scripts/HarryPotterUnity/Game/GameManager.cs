using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public class GameManager : MonoBehaviour
    {
        //TODO: Are these references kept between clients?
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        
        private IEnumerator StartGame()
        {
            Player1.Deck.Shuffle();
            Player2.Deck.Shuffle();
            yield return new WaitForSeconds(2.4f);
            Player1.DrawInitialHand();
            Player2.DrawInitialHand();

            Player1.InitTurn();
        }
        
        public void SpawnPlayer1()
        {
            // Player1 = ((GameObject)Instantiate(PlayerObject)).GetComponent<Player>();
            Player1 = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0).GetComponent<Player>();
            Player1.transform.parent = transform;

            Player1.OppositePlayer = Player2;

            Player1.Deck.InitDeck(
                DeckGenerator.GenerateDeck(new List<Lesson.LessonTypes>
                {
                    Lesson.LessonTypes.Creatures,
                    Lesson.LessonTypes.Charms
                }));

        }
        public void SpawnPlayer2()
        {
           // Player2 = ((GameObject)Instantiate(PlayerObject)).GetComponent<Player>();

            Player2 = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.Euler(0f, 0f, 180f), 0).GetComponent<Player>();
            Player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
            Player2.transform.parent = transform;

            Player2.OppositePlayer = Player1;

            Player2.Deck.InitDeck(
                DeckGenerator.GenerateDeck(new List<Lesson.LessonTypes>
                {
                    Lesson.LessonTypes.Creatures,
                    Lesson.LessonTypes.Charms,
                    Lesson.LessonTypes.Transfiguration
                }));
        }

        public void DestroyPlayers()
        {
            Destroy(Player1);
            Destroy(Player2);
        }
    }
}