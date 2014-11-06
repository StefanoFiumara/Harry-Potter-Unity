using UnityEngine;
using System.Collections;
using LessonTypes = Lesson.LessonTypes;

public abstract class GenericSpell : GenericCard {

    public LessonTypes CostType;
    public int CostAmount;

    public static readonly Vector3 SPELL_OFFSET = new Vector3(0f, 0f, -300f);

    public new void Start()
    {
        base.Start();
    }

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (_Player.UseAction())
        {
            if (_Player.nLessonsInPlay >= CostAmount && _Player.LessonTypesInPlay.Contains(CostType))
            {
                _Player._Hand.Remove(transform);
                //tween here
                //TODO: need a special tween for spell cards.
                //OnPlayAction();
                TweenToPosition();
            }
        }
    }

    protected void TweenToPosition()
    {
        //TODO: Rotate if it's being played by the opponent
        iTween.MoveTo(gameObject, iTween.Hash("time", 0.5f,
                                              "position", SPELL_OFFSET,
                                              "easetype", iTween.EaseType.easeInOutSine,
                                              "oncomplete", "PlayAndDiscard",
                                              "islocal", true,
                                              "oncompletetarget", gameObject
                                                   ));
    }

    protected void PlayAndDiscard()
    {
        Debug.Log("Tween Finished");
        OnPlayAction();
        _Player._Discard.Add(transform, 0.5f);
    }

    public abstract void OnPlayAction();


    public override void BeforeTurnAction()
    {

    }

    public override void AfterTurnAction()
    {

    }
}
