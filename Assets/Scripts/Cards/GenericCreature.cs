using UnityEngine;
using System.Collections;
using LessonTypes = Lesson.LessonTypes;

public class GenericCreature : GenericCard {

	// Use this for initialization

    public LessonTypes CostType;

    public int CostAmount;

    public int DamagePerTurn;
    public int Health;

	public new void Start ()
    {
        base.Start();
	}

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (_Player.ActionsAvailable > 0)
        {
            if(_Player.LessonsInPlay >= CostAmount && _Player.LessonTypesInPlay.Contains(CostType))
            {
                _Player._Hand.Remove(transform);
                _Player._InPlay.Add(transform, CardType);


                State = CardStates.IN_PLAY;
            }
        }
    }

    public override void BeforeTurnAction()
    {

    }

    public override void AfterTurnAction()
    {

    }


}
