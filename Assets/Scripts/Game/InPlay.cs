using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;
using CardTypes = GenericCard.CardTypes;

public class InPlay : MonoBehaviour {

    public List<Transform> Cards;

    public Player _Player;

    private const float LESSON_X_OFFSET = -160f;
    private const float LESSON_Y_OFFSET = 6f;
    private const float LESSON_Z_OFFSET = 15f;

    private const float LESSON_X_SPACING = 80f;
    private const float LESSON_Y_SPACING = 15f;

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
        }
    }

    private void AnimateLessonToBoard(Transform card)
    {
        Vector3 pos = new Vector3(LESSON_X_OFFSET, LESSON_Y_OFFSET, LESSON_Z_OFFSET);

        pos.x += (_Player.LessonsInPlay % 3) * LESSON_X_SPACING;
        pos.y -= (int)(_Player.LessonsInPlay / 3) * LESSON_Y_SPACING;
        pos.z -= (int)(_Player.LessonsInPlay / 3);

        iTween.MoveTo(card.gameObject, iTween.Hash("time", InPlayTweenTime,
                                                   "position", pos,
                                                   "easetype", iTween.EaseType.easeInOutSine,
                                                   "islocal", true
                                                   ));

        iTween.RotateTo(card.gameObject, iTween.Hash("time", InPlayTweenTime,
                                                     "z", 270f,
                                                     "easetype", iTween.EaseType.easeInOutSine
                                                     ));

        iTween.ScaleTo(card.gameObject, iTween.Hash("x", 1, "y", 1, "time", InPlayTweenTime));
    }
}
