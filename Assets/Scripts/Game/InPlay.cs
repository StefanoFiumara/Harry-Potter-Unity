using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;
using CardTypes = GenericCard.CardTypes;


public class InPlay : MonoBehaviour {

    public List<GenericCard> Cards;

    public Player _Player;

    private static readonly Vector3 LESSON_POSITION_OFFSET = new Vector3(-255f, -60f, 15f);
    private static readonly Vector3 CREATURE_POSITION_OFFSET = new Vector3(5f, -60f, 15f);

    private static readonly Vector2 LESSON_SPACING = new Vector2(80f, 15f);
    private static readonly Vector2 CREATURE_SPACING = new Vector2(80f, 36f);

    public float InPlayTweenTime;

    public void Start()
    {
        Cards = new List<GenericCard>();
    }

    public void Add(GenericCard card)
    {
        Cards.Add(card);
        card.transform.parent = transform;

        switch (card.CardType)
        {
            case CardTypes.LESSON:
                AnimateLessonToBoard(card);
                break;
            case CardTypes.CREATURE:
                AnimateCreatureToBoard(card);
                break;
        }

        var cardInfo = card.GetComponent<GenericCard>() as PersistentCard;

        //Should never be null, but we can't be too careful!
        if (cardInfo != null)
        {
            cardInfo.OnEnterInPlayAction();
        }
        else
        {
            throw new Exception("non-persistent card added to InPlay board, This should never Happen!");
        }
    }

    public void Remove(GenericCard card)
    {
        Cards.Remove(card);

        switch (card.CardType)
        {
            case CardTypes.LESSON:
                RearrangeLessons();
                break;
            case CardTypes.CREATURE:
                RearrangeCreatures();
                break;
        }
    }

    private void AnimateCreatureToBoard(GenericCard card)
    {
        Vector3 cardPosition = CREATURE_POSITION_OFFSET;

        int position = Cards.FindAll(c => c.CardType == CardTypes.CREATURE).IndexOf(card);

        cardPosition.x += (position % 3) * CREATURE_SPACING.x;
        cardPosition.y -= (int)(position / 3) * CREATURE_SPACING.y;
        cardPosition.z -= (int)(position / 3);

        Helper.TweenCardToPosition(card.transform, cardPosition, CardStates.IN_PLAY);
        if (card.State == CardStates.IN_HAND)
        {
            Helper.RotateCard(card.transform);
        }
    }

    private void AnimateLessonToBoard(GenericCard card)
    {
        Vector3 cardPosition = LESSON_POSITION_OFFSET;

        int position = Cards.FindAll(c => c.CardType == CardTypes.LESSON).IndexOf(card);

        cardPosition.x += (position % 3) * LESSON_SPACING.x;
        cardPosition.y -= (int)(position / 3) * LESSON_SPACING.y;
        cardPosition.z -= (int)(position / 3);

        Helper.TweenCardToPosition(card.transform, cardPosition, CardStates.IN_PLAY);

        if (card.State == CardStates.IN_HAND)
        {
            Helper.RotateCard(card.transform);
        }
    }

    private void RearrangeLessons()
    {
        Cards.FindAll(card => card.CardType == CardTypes.LESSON).ForEach(card => AnimateLessonToBoard(card));
    }
    private void RearrangeCreatures()
    {
        Cards.FindAll(card => card.CardType == CardTypes.CREATURE).ForEach(card => AnimateCreatureToBoard(card));
    }
}
