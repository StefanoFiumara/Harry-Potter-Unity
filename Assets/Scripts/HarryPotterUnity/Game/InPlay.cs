using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using JetBrains.Annotations;
using UnityLogWrapper;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class InPlay : CardCollection
    {
        public List<BaseCard> CardsExceptStartingCharacter
        {
            get { return Cards.Where(c => c != c.Player.Deck.StartingCharacter).ToList(); }
        } 

        private static readonly Vector3 LessonPositionOffset = new Vector3(-255f, -60f, 15f);
        private static readonly Vector3 TopRowPositionOffset = new Vector3(-255f, 0f, 15f);
        private static readonly Vector3 CreaturePositionOffset = new Vector3(5f, -60f, 15f);
        private static readonly Vector3 CharacterPositionOffset = new Vector3(-356f, -207, 15f);

        private static readonly Vector2 LessonSpacing = new Vector2(80f, 15f);
        private static readonly Vector2 TopRowSpacing = new Vector2(80f, 0f);
        private static readonly Vector2 CreatureSpacing = new Vector2(80f, 55f);
        private static readonly Vector2 CharacterSpacing = new Vector2(80f, 0f);

        public InPlay()
        {
            Cards = new List<BaseCard>();
        }

        public override void Add(BaseCard card)
        {
            Cards.Add(card);
            
            card.transform.parent = transform;

            TweenCardToPosition(card);

            MoveToThisCollection(card);

            ((IPersistentCard) card).OnEnterInPlayAction();
        }

        protected override void Remove(BaseCard card)
        {
            Cards.Remove(card);

            RearrangeCardsOfType(card.Type);

            ((IPersistentCard) card).OnExitInPlayAction();
        }

        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                Add(card);
            }
        }
        
        protected override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                Cards.Remove(card);
                ((IPersistentCard)card).OnExitInPlayAction();
            }

            foreach (var type in cardList.GroupBy(c => c.Type))
            {
                RearrangeCardsOfType(type.Key);
            }
        }

        public List<BaseCard> GetCreaturesInPlay()
        {
            return Cards.FindAll(c => c is BaseCreature);
        }

        private IEnumerable<BaseCard> GetLessonsInPlay()
        {
            return Cards.FindAll(c => c is BaseLesson);
        }

        public IEnumerable<BaseCard> GetLessonsOfType(LessonTypes type, int amount = 1)
        {
            return GetLessonsInPlay().Where(x => ((ILessonProvider)x).LessonType == type).Take(amount);
        }

        public int GetAmountOfLessonsOfType(LessonTypes type)
        {
            return GetLessonsInPlay().Count(x => ((ILessonProvider)x).LessonType == type);
        }

        private void TweenCardToPosition(BaseCard card)
        {
            var cardPosition = GetTargetPositionForCard(card);
            var tween = new MoveTween
            {
                Target = card.gameObject,
                Position = cardPosition,
                Time = 0.3f,
                Flip = FlipState.FaceUp,
                Rotate = TweenRotationType.Rotate90,
                StateAfterAnimation = State.InPlay
            };
            GameManager.TweenQueue.AddTweenToQueue( tween );
        }

        private void RearrangeCardsOfType(Type type)
        {
            GameManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards.FindAll(card => card.Type == type), GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!Cards.Contains(card)) return card.transform.localPosition;

            int position = Cards.FindAll(c => c.Type == card.Type).IndexOf(card);
            
            var cardPosition = new Vector3();

            //TODO: Violates OCP!
            switch (card.Type)
            {
                case Type.Lesson:
                    cardPosition = LessonPositionOffset;
                    cardPosition.x += (position % 3) * LessonSpacing.x;
                    cardPosition.y -= (int)(position / 3) * LessonSpacing.y;
                    break;
                case Type.Creature:
                    cardPosition = CreaturePositionOffset;
                    cardPosition.x += (position % 4) * CreatureSpacing.x;
                    cardPosition.y -= (int)(position / 4) * CreatureSpacing.y;
                    break;
                case Type.Character:
                    cardPosition = CharacterPositionOffset;
                    cardPosition.x += (position % 3) * CharacterSpacing.x;
                    break;
                case Type.Item:
                case Type.Location:
                case Type.Adventure:
                case Type.Match:
                    int topRowIndex = Cards.FindAll(c => c.Type == Type.Match || c.Type == Type.Adventure ||
                                                         c.Type == Type.Item || c.Type == Type.Location).IndexOf(card);
                    cardPosition = TopRowPositionOffset;
                    cardPosition.x += (topRowIndex % 9) * TopRowSpacing.x;
                    break;
                default:
                    Log.Error("GetTargetPositionForCard could not identify cardType");
                    break;
            }

            cardPosition.z -= (int)(position / 3);

            return cardPosition;
        }
    }
}
