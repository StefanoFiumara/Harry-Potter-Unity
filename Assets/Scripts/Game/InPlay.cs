using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;
using CardTypes = GenericCard.CardTypes;

public class InPlay : MonoBehaviour {

    public List<Transform> Cards;

    public Player _Player;

    private static readonly Vector3 LESSON_POSITION_OFFSET = new Vector3(-255f, -60f, 15f);
    private static readonly Vector3 CREATURE_POSITION_OFFSET = new Vector3(5f, -60f, 15f);

    private static readonly Vector2 LESSON_SPACING = new Vector2(80f, 15f);
    private static readonly Vector2 CREATURE_SPACING = new Vector2(80f, 36f);

    public float InPlayTweenTime;

    public void Start()
    {
        Cards = new List<Transform>();
    }

    public void Add(Transform card, CardTypes CardType)
    {
        Cards.Add(card);
        card.parent = transform;

        switch (CardType)
        {
            case CardTypes.LESSON:
                AnimateLessonToBoard(card);
                break;
            case CardTypes.CREATURE:
                AnimateCreatureToBoard(card);
                break;
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
