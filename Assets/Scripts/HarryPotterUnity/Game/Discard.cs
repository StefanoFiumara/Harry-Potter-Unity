using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Discard : MonoBehaviour
    {
        private List<GenericCard> _cards;

        private static readonly Vector2 DiscardPositionOffset = new Vector2(-355f, -30f);

        //Will use later??
        //public static readonly Vector3 PreviewOffset = new Vector3(-300f, -30f, -6f);

        [UsedImplicitly]
        public void Start () {
            _cards = new List<GenericCard>();

            if (gameObject.GetComponent<Collider>() != null) return;

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 0f);
        }

        public void Add(GenericCard card) 
        {
            _cards.Add(card);
            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  _cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;

            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPreviewPos, 0.35f, 0f, GenericCard.FlipStates.FaceUp, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.Discarded));
            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos, 0.25f, 0f, GenericCard.FlipStates.FaceUp, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.Discarded));
        }

        public void AddAll(IEnumerable<GenericCard> cards)
        {
            foreach (var card in cards)
            {
                Add(card);
            }

            AdjustCardSpacing();
        }

        public void Remove(GenericCard card)
        {
            _cards.Remove(card);
        }

        public void RemoveAll(IEnumerable<GenericCard> cards)
        {
            foreach (var card in cards)
            {
                _cards.Remove(card);
            }
        }

        public List<GenericCard> GetCards(Predicate<GenericCard> predicate)
        {
            return _cards.FindAll(predicate).ToList();
        }

        private void AdjustCardSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(_cards, GetTargetPositionForCard);
            UtilManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            int position = _cards.FindAll(c => c.CardType == card.CardType).IndexOf(card);

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -= position * 0.2f;

            return cardPos;
        }
        //TODO: OnMouseUp: View cards in discard pile
    }
}
