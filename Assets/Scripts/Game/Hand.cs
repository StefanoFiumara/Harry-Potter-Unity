using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Hand : MonoBehaviour {
    public Player _Player;

    public List<GenericCard> Cards { get; private set; }

    public static readonly Vector3 HandPreviewPosition = new Vector3(-80f, -13f, -336f);
    public static readonly Vector3 HandCardsOffset = new Vector3(-240f, -200f, 0f);
    public static readonly float Spacing = 55f;

    public Hand()
    {
        Cards = new List<GenericCard>();
    }

    public void Add(GenericCard card, bool flip = true, bool preview = true)
    {
        card.transform.parent = transform;
        AnimateCardToHand(card, flip, preview);
        Cards.Add(card);

        if (Cards.Count == 12) AdjustHandSpacing();
    }

    public void Remove(GenericCard card)
    {
        Cards.Remove(card);

        AdjustHandSpacing();
    }

    private void AdjustHandSpacing()
    {
        var shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
        
        for (var i = 0; i < Cards.Count; i++)
        {
            var cardPosition = HandCardsOffset;

            cardPosition.x += i * Hand.Spacing * shrinkFactor;
            cardPosition.z -= i;

            Helper.TweenCardToPosition(Cards[i], cardPosition, CardStates.InHand);
        }
    }

    private void AnimateCardToHand(GenericCard card, bool flip = true, bool preview = true)
    {
        var cardPosition = HandCardsOffset;

        var shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

        cardPosition.x += Cards.Count * Hand.Spacing * shrinkFactor;
        cardPosition.z -= Cards.Count;

        if (preview)
        {
            Helper.AddTweenToQueue(card, HandPreviewPosition, 0.5f, 0f, card.State, flip, false);
        }

        Helper.AddTweenToQueue(card, cardPosition, 0.5f, 0.15f, CardStates.InHand, false, card.State == CardStates.InPlay);
    }
}
