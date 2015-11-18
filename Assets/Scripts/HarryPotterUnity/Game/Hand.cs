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
    public class Hand : MonoBehaviour, ICardCollection
    {   
        public List<BaseCard> Cards { get; private set; }

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

        public void Add(BaseCard card)
        {
            Add(card, true, true);
        }

        public void Add(BaseCard card, bool preview, bool adjustSpacing)
        {
            if (adjustSpacing) AdjustHandSpacing();

            card.transform.parent = transform;
            
            Cards.Add(card);

            var flipState = _player.IsLocalPlayer ? FlipStates.FaceUp : FlipStates.FaceDown;
            
            AnimateCardToHand(card, flipState, preview);

            card.Collection.Remove(card);
            card.Collection = this;
        }
        public void AddAll(IEnumerable<BaseCard> cards)
        {
            AdjustHandSpacing();

            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                card.transform.parent = transform;
                
                Cards.Add(card);
                var flipState = _player.IsLocalPlayer ? FlipStates.FaceUp : FlipStates.FaceDown;
                AnimateCardToHand(card, flipState);
            }

            foreach (var card in cardList)
            {
                card.Collection.Remove(card);
                card.Collection = this;
            }

            AdjustHandSpacing();
        }

        public void RemoveAll(IEnumerable<BaseCard> cardsToRemove)
        {
            foreach (var card in cardsToRemove)
            {
                Cards.Remove(card);
            }
            AdjustHandSpacing();
        }
        
        public void Remove(BaseCard card)
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
            var cardPosition = HandCardsOffset;

            float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
            
            int index = Cards.IndexOf(card);
            cardPosition.x += index * SPACING * shrinkFactor;
            cardPosition.z -= index;

            return cardPosition;
        }

        private void AnimateCardToHand(BaseCard card, FlipStates flipState, bool preview = true)
        {
            var cardPosition = GetTargetPositionForCard(card);

            if (preview)
            {
                GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, HandPreviewPosition, 0.5f, 0f, flipState, RotationType.NoRotate, State.InHand));
            }

            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPosition, 0.3f, 0.1f, flipState, RotationType.NoRotate, State.InHand));
        }
    }
}
