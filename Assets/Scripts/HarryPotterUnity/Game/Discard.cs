using System;
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
    public class Discard : MonoBehaviour, ICardCollection
    {
        public List<BaseCard> Cards { get; private set; }
        
        private static readonly Vector2 DiscardPositionOffset = new Vector2(-355f, -30f);

        [UsedImplicitly]
        public void Start () {
            Cards = new List<BaseCard>();
        }

        public void Add(BaseCard card) 
        {
            card.Collection.Remove(card);
            card.Collection = this;

            Cards.Add(card);
            card.Enable(); //Do we need this?
            
            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  Cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;
            
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPreviewPos, 0.35f, 0f, FlipStates.FaceUp, RotationType.NoRotate, State.Discarded));
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos, 0.25f, 0f, FlipStates.FaceUp, RotationType.NoRotate, State.Discarded));
        }

        public void Remove(BaseCard card)
        {
            Cards.Remove(card);
        }

        public void AddAll(IEnumerable<BaseCard> cards)
        {
            AdjustCardSpacing();

            foreach (var card in cards)
            {
                Add(card);
            }
        }

        public void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Cards.Remove(card);
            }
        }

        public List<BaseCard> GetCards(Predicate<BaseCard> predicate = null)
        {
            return predicate == null ? Cards : Cards.FindAll(predicate).ToList();
        }

        public int CountCards(Func<BaseCard, bool> predicate)
        {
            return Cards.Count(predicate);
        }

        private void AdjustCardSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(new List<BaseCard>(Cards), GetTargetPositionForCard);
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            int position = Cards.IndexOf(card);

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -= position * 0.2f;
            
            return cardPos;
        }

        public List<BaseCard> GetHealableCards(int amount)
        {
            return GetCards(card => !card.Tags.Contains(Tag.Healing)).Take(amount).ToList(); 
        }
    }
}
