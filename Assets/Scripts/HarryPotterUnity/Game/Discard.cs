using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public class Discard : CardCollection
    {
        private static readonly Vector2 _discardPositionOffset = new Vector2(-355f, -30f);

        private void Start ()
        {
            Cards = new List<BaseCard>();

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(_discardPositionOffset.x, _discardPositionOffset.y, 0f);
        }

        public override void Add(BaseCard card) 
        {
            Cards.Add(card);
            card.Enable();
            
            card.transform.parent = transform;

            var cardPos = new Vector3(_discardPositionOffset.x, _discardPositionOffset.y, 16f);
            cardPos.z -=  Cards.Count * 0.2f;

            Vector3 cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;
            
            var previewTween = new MoveTween
            {
                Target = card.gameObject,
                Position = cardPreviewPos,
                Time = 0.35f,
                Flip = FlipState.FaceUp,
                Rotate = TweenRotationType.NoRotate,
                OnCompleteCallback = () => card.State = State.Discarded
            };

            var finalTween = new MoveTween
            {
                Target = card.gameObject,
                Position = cardPos,
                Time = 0.25f,
                Flip = FlipState.FaceUp,
                Rotate = TweenRotationType.NoRotate,
                OnCompleteCallback = () => card.State = State.Discarded
            };

            GameManager.TweenQueue.AddTweenToQueue( previewTween );
            GameManager.TweenQueue.AddTweenToQueue( finalTween );

            MoveToThisCollection(card);
        }

        protected override void Remove(BaseCard card)
        {
            Cards.Remove(card);
        }

        public override void AddAll(IEnumerable<BaseCard> cards)
        {

            var cardList = cards as List<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                Add(card);
            }


            AdjustCardSpacing();
        }

        protected override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Cards.Remove(card);
            }
        }
        
        private void AdjustCardSpacing()
        {
            ITweenObject tween = new AsyncMoveTween
            {
                Targets = Cards.ToList(),
                GetPosition = GetTargetPositionForCard
            };
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!Cards.Contains(card)) return card.transform.localPosition;

            int position = Cards.IndexOf(card);

            var cardPos = new Vector3(_discardPositionOffset.x, _discardPositionOffset.y, 16f);
            cardPos.z -= position * 0.2f;
            
            return cardPos;
        }

        public List<BaseCard> GetHealableCards(int amount)
        {
            return Cards.Where(c => c.HasTag(Tag.Healing) == false)
                .Take(amount).ToList();
        }
    }
}
