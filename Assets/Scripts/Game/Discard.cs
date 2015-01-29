using System.Collections.Generic;
using Assets.Scripts.Cards;
using UnityEngine;
using CardStates = Assets.Scripts.Cards.GenericCard.CardStates;

namespace Assets.Scripts.Game
{
    public class Discard : MonoBehaviour {

        private List<GenericCard> _cards; //TODO: Does this need to be private?

        public static readonly Vector2 DiscardPositionOffset = new Vector2(-355f, -30f);

        public static readonly Vector3 PreviewOffset = new Vector3(-300f, -30f, -6f);

        public void Start () {
            _cards = new List<GenericCard>();

            if (gameObject.collider == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(50f, 70f, 1f);
                col.center = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 0f);
            }
        }

        public void Add(GenericCard card) 
        {
            _cards.Add(card);
            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  _cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;

            Helper.AddTweenToQueue(card, cardPreviewPos, 0.35f, 0f, CardStates.Discarded, card.State == CardStates.InDeck, card.State == CardStates.InPlay);
            Helper.AddTweenToQueue(card, cardPos, 0.25f, 0f, CardStates.Discarded, false, false);
        }

        //TODO: OnMouseUp: View cards in discard pile
    }
}
