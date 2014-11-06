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
                AnimateLessonToBoard(card.transform);
                break;
            case CardTypes.CREATURE:
                AnimateCreatureToBoard(card.transform);
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

    private void AnimateCreatureToBoard(Transform card)
    {
        Vector3 cardPosition = CREATURE_POSITION_OFFSET;

        cardPosition.x += (_Player.nCreaturesInPlay % 3) * CREATURE_SPACING.x;
        cardPosition.y -= (int)(_Player.nCreaturesInPlay / 3) * CREATURE_SPACING.y;
        cardPosition.z -= (int)(_Player.nCreaturesInPlay / 3);

        Helper.TweenCardToPosition(card, cardPosition, CardStates.IN_PLAY);
        Helper.RotateCard(card);
    }

    private void AnimateLessonToBoard(Transform card)
    {
        Vector3 cardPosition = LESSON_POSITION_OFFSET;

        cardPosition.x += (_Player.nLessonsInPlay % 3) * LESSON_SPACING.x;
        cardPosition.y -= (int)(_Player.nLessonsInPlay / 3) * LESSON_SPACING.y;
        cardPosition.z -= (int)(_Player.nLessonsInPlay / 3);

        Helper.TweenCardToPosition(card, cardPosition, CardStates.IN_PLAY);
        Helper.RotateCard(card);
    }
}
