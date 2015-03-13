using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Hand : MonoBehaviour {
        
        public List<GenericCard> Cards { get; private set; }

        private Player _player;

        private static readonly Vector3 HandPreviewPosition = new Vector3(-80f, -13f, -336f);

        public static readonly Vector3 HandCardsOffset = new Vector3(-240f, -200f, 0f);

        public const float Spacing = 55f;

        public Hand()
        {
            Cards = new List<GenericCard>();
        }

        [UsedImplicitly]
        public void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
        }

        public void Add(GenericCard card, bool preview = true)
        {
            var shouldFlip = card.FlipState == GenericCard.FlipStates.FaceUp && !_player.IsLocalPlayer || 
                             card.FlipState == GenericCard.FlipStates.FaceDown && _player.IsLocalPlayer;

            card.transform.parent = transform;

            if (Cards.Count == 12) AdjustHandSpacing();
            AnimateCardToHand(card, shouldFlip, preview);

            Cards.Add(card);
        }

        public void Remove(GenericCard card)
        {
            Cards.Remove(card);

            AdjustHandSpacing();
        }

        private void AdjustHandSpacing()
        {
            UtilManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards, GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            var shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
            var cardPosition = HandCardsOffset;

            var index = Cards.IndexOf(card);
            cardPosition.x += index * Spacing * shrinkFactor;
            cardPosition.z -= index;

            return cardPosition;
        }

        private void AnimateCardToHand(GenericCard card, bool flip = true, bool preview = true)
        {
            var cardPosition = HandCardsOffset;

            var shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

            cardPosition.x += Cards.Count * Spacing * shrinkFactor;
            cardPosition.z -= Cards.Count;

            if (preview)
            {
                UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, HandPreviewPosition, 0.5f, 0f, flip, TweenQueue.RotationType.NoRotate, card.State));
            }

            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPosition, 0.3f, 0.1f, !preview && flip, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.InHand));
        }
    }
}
