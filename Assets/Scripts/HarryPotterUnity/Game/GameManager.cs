using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HarryPotterUnity.Cards;
using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Game
{
    public class GameManager : MonoBehaviour
    {
        public Player Player1, Player2;
        
        void Start()
        {
             //TODO: Instantiate Player 1 and Player 2 (with generated Decks) in Awake()
             Player1.OppositePlayer = Player2;
             Player2.OppositePlayer = Player1;

            Player1.Deck.InitDeck(
                DeckGenerator.GenerateDeck(new List<Lesson.LessonTypes>
                {
                    Lesson.LessonTypes.Creatures,
                    Lesson.LessonTypes.Charms
                }));
            
            Player2.Deck.InitDeck(
                DeckGenerator.GenerateDeck(new List<Lesson.LessonTypes>
                {
                    Lesson.LessonTypes.Potions,
                    Lesson.LessonTypes.Quidditch,
                    Lesson.LessonTypes.Transfiguration
                }));

            Player1.Deck.Shuffle();
            Player2.Deck.Shuffle();

            StartCoroutine(StartGame());
        }

        IEnumerator StartGame()
        {
            yield return new WaitForSeconds(2.4f);
            Player1.DrawInitialHand();
            Player2.DrawInitialHand();

            Player1.InitTurn();
        } 
    }
}