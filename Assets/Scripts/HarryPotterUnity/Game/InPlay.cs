using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class InPlay : MonoBehaviour {
        public List<GenericCard> Cards { get; private set; }

        private static readonly Vector3 LessonPositionOffset = new Vector3(-255f, -60f, 15f);
        private static readonly Vector3 CreaturePositionOffset = new Vector3(5f, -60f, 15f);

        private static readonly Vector2 LessonSpacing = new Vector2(80f, 15f);
        private static readonly Vector2 CreatureSpacing = new Vector2(80f, 36f);


        public InPlay()
        {
            Cards = new List<GenericCard>();
        }

        public void Add(GenericCard card)
        {
            Cards.Add(card);
            card.transform.parent = transform;

            TweenCardToPosition(card);

            ((IPersistentCard) card).OnEnterInPlayAction();
        }

        public void Remove(GenericCard card)
        {
            Cards.Remove(card);

            RearrangeCardsOfType(card.CardType);

            ((IPersistentCard) card).OnExitInPlayAction();
        }

        public List<GenericCard> GetCreaturesInPlay()
        {
            return Cards.FindAll(c => c.CardType == GenericCard.CardTypes.Creature);
        }

        private IEnumerable<GenericCard> GetLessonsInPlay()
        {
            return Cards.FindAll(c => c.CardType == GenericCard.CardTypes.Lesson);
        }

        public Lesson GetLessonOfType(Lesson.LessonTypes type)
        {
            return GetLessonsInPlay().First(x => ((Lesson)x).LessonType == type) as Lesson;
        }

        public int GetAmountOfLessonsOfType(Lesson.LessonTypes type)
        {
            return GetLessonsInPlay().Count(x => ((Lesson)x).LessonType == type);
        }

        private void TweenCardToPosition(GenericCard card)
        {
            var cardPosition = GetTargetPositionForCard(card);

            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject,
                cardPosition,
                0.3f,
                0f,
                GenericCard.FlipStates.FaceUp, 
                TweenQueue.RotationType.Rotate90,
                GenericCard.CardStates.InPlay));
        }

        private void RearrangeCardsOfType(GenericCard.CardTypes type)
        {
            UtilManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards.FindAll(card => card.CardType == type), GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            int position = Cards.FindAll(c => c.CardType == card.CardType).IndexOf(card);

            var cardPosition = new Vector3();

            //TODO: Violates OCP!
            switch (card.CardType)
            {
                case GenericCard.CardTypes.Lesson:
                    cardPosition = LessonPositionOffset;
                    cardPosition.x += (position % 3) * LessonSpacing.x;
                    cardPosition.y -= (int)(position / 3) * LessonSpacing.y;
                    break;
                case GenericCard.CardTypes.Creature:
                    cardPosition = CreaturePositionOffset;
                    cardPosition.x += (position % 3) * CreatureSpacing.x;
                    cardPosition.y -= (int)(position / 3) * CreatureSpacing.y;
                    break;
                default:
                    Debug.Log("Warning: GetTargetPositionForCard could not identify cardType");
                    break;
            }

            cardPosition.z -= (int)(position / 3);

            return cardPosition;
        }
    }
}
