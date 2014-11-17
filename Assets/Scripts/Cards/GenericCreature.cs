using UnityEngine;
using System.Collections;
using LessonTypes = Lesson.LessonTypes;

public class GenericCreature : GenericCard, PersistentCard {

    public LessonTypes CostType;

    public int CostAmount;

    public int DamagePerTurn;
    public int Health;

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (_Player.CanUseAction())
        {
            if(_Player.nLessonsInPlay >= CostAmount && _Player.LessonTypesInPlay.Contains(CostType))
            {
                _Player.UseAction();
                _Player._Hand.Remove(this);
                _Player._InPlay.Add(this);
            }
        }
    }

    public void OnEnterInPlayAction()
    {
        _Player.nCreaturesInPlay++;
        _Player.DamagePerTurn += DamagePerTurn;

        State = CardStates.IN_PLAY;
    }

    public void OnExitInPlayAction()
    {
        _Player.nCreaturesInPlay--;
        _Player.DamagePerTurn -= DamagePerTurn;
    }

    //Generic Creatures don't do anything special on these actions
    public void OnInPlayBeforeTurnAction() { }
    public void OnInPlayAfterTurnAction() { }
    public void OnSelectedAction() { }
  
}
