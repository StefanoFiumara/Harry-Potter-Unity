using System.Collections.Generic;
using Assets.Scripts.HarryPotterUnity.Cards;
using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;
using CardStates = Assets.Scripts.HarryPotterUnity.Cards.GenericCard.CardStates;
using CardTypes = Assets.Scripts.HarryPotterUnity.Cards.GenericCard.CardTypes;


namespace Assets.Scripts.HarryPotterUnity.Game
{
    public class InPlay : MonoBehaviour {

        public List<GenericCard> Cards { get; private set; }

       // public Player Player;

        private static readonly Vector3 LessonPositionOffset = new Vector3(-255f, -60f, 15f);
        private static readonly Vector3 CreaturePositionOffset = new Vector3(5f, -60f, 15f);

        private static readonly Vector2 LessonSpacing = new Vector2(80f, 15f);
        private static readonly Vector2 CreatureSpacing = new Vector2(80f, 36f);

        public float InPlayTweenTime;

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
                case CardTypes.Lesson:
                    AnimateLessonToBoard(card);
                    break;
                case CardTypes.Creature:
                    AnimateCreatureToBoard(card);
                    break;
            }

            (card as IPersistentCard).OnEnterInPlayAction();
        }

        public void Remove(GenericCard card)
        {
            Cards.Remove(card);

            switch (card.CardType)
            {
                case CardTypes.Lesson:
                    RearrangeLessons();
                    break;
                case CardTypes.Creature:
                    RearrangeCreatures();
                    break;
            }

            (card as IPersistentCard).OnExitInPlayAction();
        }

        public List<GenericCard> GetCreaturesInPlay()
        {
            return Cards.FindAll(c => c.CardType == CardTypes.Creature);
        }

        public List<GenericCard> GetLessonsInPlay()
        {
            return Cards.FindAll(c => c.CardType == CardTypes.Lesson);
        }

        private void AnimateCreatureToBoard(GenericCard card)
        {
            Vector3 cardPosition = GetTargetPositionForCard(card);
            UtilManager.AddTweenToQueue(card, cardPosition, 0.3f, 0f, CardStates.InPlay, false, card.State == CardStates.InHand);
        }

        private void AnimateLessonToBoard(GenericCard card)
        {
            Vector3 cardPosition = GetTargetPositionForCard(card);
            UtilManager.AddTweenToQueue(card, cardPosition, 0.3f, 0f, CardStates.InPlay, false, card.State == CardStates.InHand);
        }

    

        private void RearrangeLessons()
        {
            Cards.FindAll(card => card.CardType == CardTypes.Lesson).ForEach(card => 
            {
                Vector3 cardPosition = GetTargetPositionForCard(card);
                UtilManager.TweenCardToPosition(card, cardPosition, CardStates.InPlay);
            });
        }
        private void RearrangeCreatures()
        {
            Cards.FindAll(card => card.CardType == CardTypes.Creature).ForEach(card => 
            {
                Vector3 cardPosition = GetTargetPositionForCard(card);
                UtilManager.TweenCardToPosition(card, cardPosition, CardStates.InPlay);
            });
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            int position = Cards.FindAll(c => c.CardType == card.CardType).IndexOf(card);

            Vector3 cardPosition = new Vector3();

            switch (card.CardType)
            {
                case CardTypes.Lesson:
                    cardPosition = LessonPositionOffset;
                    cardPosition.x += (position % 3) * LessonSpacing.x;
                    cardPosition.y -= (int)(position / 3) * LessonSpacing.y;
                    break;
                case CardTypes.Creature:
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
