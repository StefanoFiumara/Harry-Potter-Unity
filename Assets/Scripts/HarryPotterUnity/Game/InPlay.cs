﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
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
        private static readonly Vector3 ItemPositionOffset = new Vector3(-255f, 0f, 15f);
        private static readonly Vector3 CreaturePositionOffset = new Vector3(5f, -60f, 15f);
        private static readonly Vector3 CharacterPositionOffset = new Vector3(-356f, -207, 15f);

        private static readonly Vector2 LessonSpacing = new Vector2(80f, 15f);
        private static readonly Vector2 ItemSpacing = new Vector2(80f, 0f);
        private static readonly Vector2 CreatureSpacing = new Vector2(80f, 36f);
        private static readonly Vector2 CharacterSpacing = new Vector2(80f, 0f);


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

            RearrangeCardsOfType(card.Type);

            ((IPersistentCard) card).OnExitInPlayAction();
        }

        public void RemoveAll(List<GenericCard> cards)
        {
            foreach (var card in cards)
            {
                Cards.Remove(card);
                ((IPersistentCard)card).OnExitInPlayAction();
            }

            foreach (var type in cards.GroupBy(c => c.Type))
            {
                RearrangeCardsOfType(type.Key);
            }
        }

        public List<GenericCard> GetCreaturesInPlay()
        {
            return Cards.FindAll(c => c.Type == GenericCard.CardType.Creature);
        }

        private IEnumerable<GenericCard> GetLessonsInPlay()
        {
            return Cards.FindAll(c => c.Type == GenericCard.CardType.Lesson);
        }

        public IEnumerable<GenericCard> GetLessonsOfType(LessonTypes type, int amount = 1)
        {
            return GetLessonsInPlay().Where(x => ((ILessonProvider)x).LessonType == type).Take(amount);
        }

        public int GetAmountOfLessonsOfType(LessonTypes type)
        {
            return GetLessonsInPlay().Count(x => ((ILessonProvider)x).LessonType == type);
        }

        private void TweenCardToPosition(GenericCard card)
        {
            var cardPosition = GetTargetPositionForCard(card);

            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject,
                cardPosition, 
                0.3f,
                0f,
                GenericCard.FlipStates.FaceUp, 
                TweenQueue.RotationType.Rotate90,
                GenericCard.CardStates.InPlay));
        }

        private void RearrangeCardsOfType(GenericCard.CardType type)
        {
            GameManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards.FindAll(card => card.Type == type), GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            int position = Cards.FindAll(c => c.Type == card.Type).IndexOf(card);

            var cardPosition = new Vector3();

            //TODO: Violates OCP!
            switch (card.Type)
            {
                case GenericCard.CardType.Lesson:
                    cardPosition = LessonPositionOffset;
                    cardPosition.x += (position % 3) * LessonSpacing.x;
                    cardPosition.y -= (int)(position / 3) * LessonSpacing.y;
                    break;
                case GenericCard.CardType.Creature:
                    cardPosition = CreaturePositionOffset;
                    cardPosition.x += (position % 3) * CreatureSpacing.x;
                    cardPosition.y -= (int)(position / 3) * CreatureSpacing.y;
                    break;
                case GenericCard.CardType.Item:
                    cardPosition = ItemPositionOffset;
                    cardPosition.x += (position % 9) * ItemSpacing.x;
                    break;
                case GenericCard.CardType.Character:
                    cardPosition = CharacterPositionOffset;
                    cardPosition.x += (position%3) * CharacterSpacing.x;
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
