using System.Collections.Generic;
using Assets.Scripts.HarryPotterUnity.Cards;
using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Game
{
    public class Player : MonoBehaviour {

        public Player OppositePlayer { get; set; }
        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public InPlay InPlay { get; private set; }
        public Discard Discard { get; private set; }

        public List<Lesson.LessonTypes> LessonTypesInPlay { get; private set; }

        public int ActionsAvailable { get; private set; }

        public int CreaturesInPlay { get; set; }
        public int DamagePerTurn { get; set; }
        public int AmountLessonsInPlay { get; set; }

        public void Awake()
        {
            LessonTypesInPlay = new List<Lesson.LessonTypes>();
            ActionsAvailable = 0;
            AmountLessonsInPlay = 0;

            Hand = transform.GetComponentInChildren<Hand>();
            Deck = transform.GetComponentInChildren<Deck>();
            InPlay = transform.GetComponentInChildren<InPlay>();
            Discard = transform.GetComponentInChildren<Discard>();
        }

        public void UseAction()
        {
            ActionsAvailable--;

            if (ActionsAvailable > 0) return;
            ActionsAvailable = 0;
            //AfterTurnAction happens here
            InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayAfterTurnAction());
            OppositePlayer.InitTurn();
        }

        public void AddAction()
        {
            ActionsAvailable++;
        }

        public void InitTurn()
        {
            //BeforeTurnAction happens here
            InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayBeforeTurnAction());

            Deck.DrawCard();
            ActionsAvailable += 2;

            //Creatures do damage here
            InPlay.GetCreaturesInPlay()
                .ForEach(card => OppositePlayer
                    .TakeDamage( ( (GenericCreature)card).DamagePerTurn) );
        }

        public bool CanUseAction()
        {
            return ActionsAvailable > 0;
        }

        public void DrawInitialHand()
        {
            //TODO: Needs cleanup
            for (var i = 0; i < 7; i++)
            {
                var card = Deck.TakeTopCard();
                var cardPosition = Hand.HandCardsOffset;

                var shrinkFactor = Hand.Cards.Count >= 12 ? 0.5f : 1f;

                cardPosition.x += Hand.Cards.Count * Hand.Spacing * shrinkFactor;
                cardPosition.z -= Hand.Cards.Count;

                Hand.Cards.Add(card);
                card.transform.parent = Hand.transform;

                UtilManager.AddTweenToQueue(card, cardPosition, 0.3f, 0f, GenericCard.CardStates.InHand, true, false);
            }       
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;

            for (var i = 0; i < amount; i++)
            {
                var card = Deck.TakeTopCard();

                if (card == null)
                {
                    //TODO: Show game over message here.
                    Debug.Log("Game Over");
                    break;
                }
                Discard.Add(card);
            }
        }

        public void UpdateLessonTypesInPlay()
        {
            LessonTypesInPlay = new List<Lesson.LessonTypes>();

            var lessons = InPlay.Cards.FindAll(card => card is Lesson);

            foreach (var genericCard in lessons)
            {
                var lessonCard = (Lesson) genericCard;
                if (!LessonTypesInPlay.Contains(lessonCard.LessonType))
                {
                    LessonTypesInPlay.Add(lessonCard.LessonType);
                }
            }
        }

        public void DisableAllCards()
        {
            Deck.gameObject.layer = UtilManager.IgnoreRaycastLayer;
            UtilManager.DisableCards(Hand.Cards);
            UtilManager.DisableCards(InPlay.Cards);
        }

        public void EnableAllCards()
        {
            Deck.gameObject.layer = UtilManager.DeckLayer;
            UtilManager.EnableCards(Hand.Cards);
            UtilManager.EnableCards(InPlay.Cards);
        }
    }
}
