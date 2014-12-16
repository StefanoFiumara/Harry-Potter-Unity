using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Hand : MonoBehaviour {
    public Player _Player;

    public List<GenericCard> Cards
    {
        get { return _Cards; }
    }

    private List<GenericCard> _Cards = new List<GenericCard>();

    public static readonly Vector3 HAND_PREVIEW_POSITION = new Vector3(-80f, -13f, -336f);
    public static readonly Vector3 HAND_CARDS_OFFSET = new Vector3(-240f, -200f, 0f);
    public static readonly float SPACING = 55f;

    public void Add(GenericCard card, bool flip = true, bool preview = true)
    {
        card.transform.parent = transform;
        AnimateCardToHand(card, flip, preview);
        _Cards.Add(card);

        if (_Cards.Count == 12) AdjustHandSpacing();
    }

    public void Remove(GenericCard card)
    {
        _Cards.Remove(card);

        AdjustHandSpacing();
    }

    private void AdjustHandSpacing()
    {
        Vector3 cardPosition;
        
        float shrinkFactor = _Cards.Count >= 12 ? 0.5f : 1f;
        
        for (int i = 0; i < _Cards.Count; i++)
        {
            cardPosition = HAND_CARDS_OFFSET;

            cardPosition.x += i * Hand.SPACING * shrinkFactor;
            cardPosition.z -= i;

            Helper.TweenCardToPosition(Cards[i], cardPosition, CardStates.IN_HAND);
        }
    }

    private void AnimateCardToHand(GenericCard card, bool flip = true, bool preview = true)
    {
        Vector3 cardPosition = HAND_CARDS_OFFSET;

        float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

        cardPosition.x += Cards.Count * Hand.SPACING * shrinkFactor;
        cardPosition.z -= Cards.Count;

        if (preview)
        {
            Helper.AddTweenToQueue(card, HAND_PREVIEW_POSITION, 0.5f, 0f, card.State, flip, false);
        }

        Helper.AddTweenToQueue(card, cardPosition, 0.5f, 0.15f, CardStates.IN_HAND, false, card.State == CardStates.IN_PLAY);
    }
}
