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

        [UsedImplicitly]
        public void Start () {
            _cards = new List<GenericCard>();

            /*
            if (gameObject.GetComponent<Collider>() != null) return;
            
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 0f);
             * */
        }

        public void Add(GenericCard card) 
        {
            _cards.Add(card);
            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  _cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;

            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPreviewPos, 0.35f, 0f, GenericCard.FlipStates.FaceUp, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.Discarded));
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos, 0.25f, 0f, GenericCard.FlipStates.FaceUp, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.Discarded));
        }

        public void AddAll(IEnumerable<GenericCard> cards)
        {
            AdjustCardSpacing();

            foreach (var card in cards)
            {
                Add(card);
            }
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

        public int CountCards(Func<GenericCard, bool> predicate)
        {
            return _cards.Count(predicate);
        }

        private void AdjustCardSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(new List<GenericCard>(_cards), GetTargetPositionForCard);
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            int position = _cards.FindAll(c => c.Type == card.Type).IndexOf(card);

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -= position * 0.2f;

            return cardPos;
        }
        //TODO: OnMouseUp: View cards in discard pile
    }
}
