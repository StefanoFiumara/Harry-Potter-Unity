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

        (card as PersistentCard).OnEnterInPlayAction();
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

        (card as PersistentCard).OnExitInPlayAction();
    }

    public List<GenericCard> GetCreaturesInPlay()
    {
        return Cards.FindAll(c => c.CardType == CardTypes.CREATURE);
    }

    public List<GenericCard> GetLessonsInPlay()
    {
        return Cards.FindAll(c => c.CardType == CardTypes.LESSON);
    }

    private void AnimateCreatureToBoard(GenericCard card)
    {
        Vector3 cardPosition = GetTargetPositionForCard(card);
        Helper.AddTweenToQueue(card, cardPosition, 0.3f, 0f, CardStates.IN_PLAY, false, card.State == CardStates.IN_HAND);
    }

    private void AnimateLessonToBoard(GenericCard card)
    {
        Vector3 cardPosition = GetTargetPositionForCard(card);
        Helper.AddTweenToQueue(card, cardPosition, 0.3f, 0f, CardStates.IN_PLAY, false, card.State == CardStates.IN_HAND);
    }

    

    private void RearrangeLessons()
    {
        Cards.FindAll(card => card.CardType == CardTypes.LESSON).ForEach(card => 
        {
            Vector3 cardPosition = GetTargetPositionForCard(card);
            Helper.TweenCardToPosition(card, cardPosition, CardStates.IN_PLAY);
        });
    }
    private void RearrangeCreatures()
    {
        Cards.FindAll(card => card.CardType == CardTypes.CREATURE).ForEach(card => 
        {
            Vector3 cardPosition = GetTargetPositionForCard(card);
            Helper.TweenCardToPosition(card, cardPosition, CardStates.IN_PLAY);
        });
    }

    private Vector3 GetTargetPositionForCard(GenericCard card)
    {
        int position = Cards.FindAll(c => c.CardType == card.CardType).IndexOf(card);

        Vector3 cardPosition = new Vector3();

        switch (card.CardType)
        {
            case CardTypes.LESSON:
                cardPosition = LESSON_POSITION_OFFSET;
                cardPosition.x += (position % 3) * LESSON_SPACING.x;
                cardPosition.y -= (int)(position / 3) * LESSON_SPACING.y;
                break;
            case CardTypes.CREATURE:
                cardPosition = CREATURE_POSITION_OFFSET;
                cardPosition.x += (position % 3) * CREATURE_SPACING.x;
                cardPosition.y -= (int)(position / 3) * CREATURE_SPACING.y;
                break;
            default:
                Debug.Log("Warning: GetTargetPositionForCard could not identify cardType");
                break;
        }

        cardPosition.z -= (int)(position / 3);

        return cardPosition;
    }
}
