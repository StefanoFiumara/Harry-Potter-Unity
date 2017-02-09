using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public class Hand : CardCollection
    {   
        private Player _player;

        private static readonly Vector3 _handPreviewPosition = new Vector3(-80f, -13f, -336f);

        private static readonly Vector3 _handCardsOffset = new Vector3(-240f, -200f, 0f);

        private const float SPACING = 55f;

        private void Awake()
        {
            this._player = this.transform.GetComponentInParent<Player>();
        }

        public override void Add(BaseCard card)
        {
            this.Add(card, preview: true, adjustSpacing: true);
        }

        public void Add(BaseCard card, bool preview, bool adjustSpacing)
        {
            if (adjustSpacing) this.AdjustHandSpacing();

            card.transform.parent = this.transform;

            this.Cards.Add(card);

            //TODO: Add option to show the card to both players before putting it into the player's hand
            var flipState = this._player.IsLocalPlayer ? FlipState.FaceUp : FlipState.FaceDown;

            this.AnimateCardToHand(card, flipState, preview);

            this.MoveToThisCollection(card);
        }
        
        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            this.AdjustHandSpacing();

            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                card.transform.parent = this.transform;

                this.Cards.Add(card);
                var flipState = this._player.IsLocalPlayer ? FlipState.FaceUp : FlipState.FaceDown;
                this.AnimateCardToHand(card, flipState);
            }

            this.MoveToThisCollection(cardList);

            this.AdjustHandSpacing();
        }

        protected override void RemoveAll(IEnumerable<BaseCard> cardsToRemove)
        {
            foreach (var card in cardsToRemove)
            {
                this.Cards.Remove(card);
            }

            this.AdjustHandSpacing();
        }

        protected override void Remove(BaseCard card)
        {
            this.RemoveAll(new[] { card });
        }

        public void AdjustHandSpacing()
        {
            var tween = new AsyncMoveTween
            {
                Targets = this.Cards.ToList(),
                GetPosition = this.GetTargetPositionForCard
            };
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!this.Cards.Contains(card)) return card.transform.localPosition;

            var cardPosition = _handCardsOffset;

            float shrinkFactor = this.Cards.Count >= 12 ? 0.5f : 1f;
            
            int index = this.Cards.IndexOf(card);
            cardPosition.x += index * SPACING * shrinkFactor;
            cardPosition.z -= index;

            return cardPosition;
        }

        private void AnimateCardToHand(BaseCard card, FlipState flipState, bool preview = true)
        {
            var cardPosition = this.GetTargetPositionForCard(card);

            if (preview)
            {
                var previewTween = new MoveTween
                {
                    Target = card.gameObject,
                    Position = _handPreviewPosition,
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
