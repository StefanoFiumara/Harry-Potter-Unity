using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Hand : CardCollection
    {   
        private Player _player;

        private static readonly Vector3 HandPreviewPosition = new Vector3(-80f, -13f, -336f);

        private static readonly Vector3 HandCardsOffset = new Vector3(-240f, -200f, 0f);

        private const float SPACING = 55f;

        public Hand()
        {
            Cards = new List<BaseCard>();
        }

        [UsedImplicitly]
        public void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
        }

        public override void Add(BaseCard card)
        {
            Add(card, preview: true, adjustSpacing: true);
        }

        public void Add(BaseCard card, bool preview, bool adjustSpacing)
        {
            if (adjustSpacing) AdjustHandSpacing();

            card.transform.parent = transform;
            
            Cards.Add(card);

            var flipState = _player.IsLocalPlayer ? FlipState.FaceUp : FlipState.FaceDown;
            
            AnimateCardToHand(card, flipState, preview);

            MoveToThisCollection(card);
        }
        
        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            AdjustHandSpacing();

            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                card.transform.parent = transform;
                
                Cards.Add(card);
                var flipState = _player.IsLocalPlayer ? FlipState.FaceUp : FlipState.FaceDown;
                AnimateCardToHand(card, flipState);
            }

            foreach (var card in cardList)
            {
                MoveToThisCollection(card);
            }

            AdjustHandSpacing();
        }

        protected override void RemoveAll(IEnumerable<BaseCard> cardsToRemove)
        {
            foreach (var card in cardsToRemove)
            {
                Cards.Remove(card);
            }

            AdjustHandSpacing();
        }

        protected override void Remove(BaseCard card)
        {
            RemoveAll(new[] { card });
        }

        public void AdjustHandSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(new List<BaseCard>(Cards), GetTargetPositionForCard);
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!Cards.Contains(card)) return card.transform.localPosition;

            var cardPosition = HandCardsOffset;

            float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
            
            int index = Cards.IndexOf(card);
            cardPosition.x += index * SPACING * shrinkFactor;
            cardPosition.z -= index;

            return cardPosition;
        }

        private void AnimateCardToHand(BaseCard card, FlipState flipState, bool preview = true)
        {
            var cardPosition = GetTargetPositionForCard(card);

            if (preview)
            {
                var previewTween = new MoveTween
                {
                    Target = card.gameObject,
                    Position = HandPreviewPosition,
                    Time = 0.5f,
                    Flip = flipState,
                    Rotate = TweenRotationType.NoRotate,
                    OnCompleteCallback = () => card.State = State.InHand
                };
                GameManager.TweenQueue.AddTweenToQueue( previewTween );
            }

            var finalTween = new MoveTween
            {
                Target = card.gameObject,
                Position = cardPosition,
                Time = 0.3f,
                Delay = 0.1f,
                Flip = flipState,
                Rotate = TweenRotationType.NoRotate,
                OnCompleteCallback = () => card.State = State.InHand
            };
            GameManager.TweenQueue.AddTweenToQueue( finalTween );
        }
    }
}
