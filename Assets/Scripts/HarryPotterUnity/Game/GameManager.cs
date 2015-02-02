using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HarryPotterUnity.Cards;
using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] protected GameObject PlayerObject;

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        
        void Start()
        {
            SpawnPlayers();
        }

        IEnumerator StartGame()
        {
            yield return new WaitForSeconds(2.4f);
            Player1.DrawInitialHand();
            Player2.DrawInitialHand();

            Player1.InitTurn();
        }
        public void SpawnPlayers()
        {
            Player1 = ((GameObject) Instantiate(PlayerObject)).GetComponent<Player>();
            Player2 = ((GameObject) Instantiate(PlayerObject)).GetComponent<Player>();

            Player2.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            Player1.transform.parent = Player2.transform.parent = transform;

            Player1.OppositePlayer = Player2;
            Player2.OppositePlayer = Player1;

            //temporary lesson types, switch out with player input when we build the menu
            Player1.Deck.InitDeck(
                DeckGenerator.GenerateDeck(new List<Lesson.LessonTypes>
                {
                    Lesson.LessonTypes.Creatures,
                    Lesson.LessonTypes.Charms
                }));

            Player2.Deck.InitDeck(
                DeckGenerator.GenerateDeck(new List<Lesson.LessonTypes>
                {
                    Lesson.LessonTypes.Creatures,
                    Lesson.LessonTypes.Charms,
                    Lesson.LessonTypes.Transfiguration
                }));

            Player1.Deck.Shuffle();
            Player2.Deck.Shuffle();

            StartCoroutine(StartGame());
        }
    }
}