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

        private Player _player;

        [UsedImplicitly]
        public void Start()
        {
            _player = transform.GetComponentInParent<Player>();
        }
        public InPlay()
        {
            Cards = new List<GenericCard>();
        }

        public void Add(GenericCard card)
        {
            Cards.Add(card);
            card.transform.parent = transform;

            switch (card.CardType)
            {
                case GenericCard.CardTypes.Lesson:
                    AnimateLessonToBoard(card);
                    break;
                case GenericCard.CardTypes.Creature:
                    AnimateCreatureToBoard(card);
                    break;
            }

            ((IPersistentCard) card).OnEnterInPlayAction();
        }

        public void Remove(GenericCard card)
        {
            Cards.Remove(card);

            switch (card.CardType)
            {
                case GenericCard.CardTypes.Lesson:
                    RearrangeLessons();
                    break;
                case GenericCard.CardTypes.Creature:
                    RearrangeCreatures();
                    break;
            }

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

        //TODO: AnimateCreature and Lesson to board have the exact same code, can we simplify this?
        private void AnimateCreatureToBoard(GenericCard card)
        {
            var cardPosition = GetTargetPositionForCard(card);
            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, 
                                                                 cardPosition, 
                                                                 0.3f, 
                                                                 0f, 
                                                                 !_player.IsLocalPlayer, 
                                                                 TweenQueue.RotationType.Rotate90, 
                                                                 GenericCard.CardStates.InPlay));
        }

        private void AnimateLessonToBoard(GenericCard card)
        {
            var cardPosition = GetTargetPositionForCard(card);
            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject,
                                                                 cardPosition,
                                                                 0.3f,
                                                                 0f,
                                                                 !_player.IsLocalPlayer,
                                                                 TweenQueue.RotationType.Rotate90,
                                                                 GenericCard.CardStates.InPlay));
        }

        //TODO: Again, these two functions are very similar and could be combined by passing the type.
        private void RearrangeLessons()
        {
            UtilManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards.FindAll(card => card.CardType == GenericCard.CardTypes.Lesson), GetTargetPositionForCard));
        }
        private void RearrangeCreatures()
        {
            UtilManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards.FindAll(card => card.CardType == GenericCard.CardTypes.Creature), GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            var position = Cards.FindAll(c => c.CardType == card.CardType).IndexOf(card);

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
