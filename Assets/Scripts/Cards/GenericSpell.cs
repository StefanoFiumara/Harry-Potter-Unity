using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;

public abstract class GenericSpell : GenericCard {

    public LessonTypes CostType;
    public int CostAmount;

    public int nInputRequired;

    public static readonly Vector3 SPELL_OFFSET = new Vector3(0f, 0f, -400f);

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (_Player.UseAction())
        {
            if (_Player.nLessonsInPlay >= CostAmount && _Player.LessonTypesInPlay.Contains(CostType))
            {
                if (MeetsAdditionalPlayRequirements())
                {
                    _Player._Hand.Remove(this);
                    PlaySpell();
                }
            }
        }
    }

    protected void PlaySpell()
    {
        //TODO: Rotate if it's being played by the opponent
        iTween.MoveTo(gameObject, iTween.Hash("time", 0.5f,
                                              "position", SPELL_OFFSET,
                                              "easetype", iTween.EaseType.easeInOutSine,
                                              "oncomplete", "ExecuteActionAndDiscard",
                                              "islocal", true,
                                              "oncompletetarget", gameObject
                                                   ));
    }

    protected void ExecuteActionAndDiscard()
    {
        OnPlayAction();
        _Player._Discard.Add(this, 1f);
    }

    protected IEnumerator WaitForPlayerInput(List<GenericCard> selectedCards)
    {
        while (selectedCards.Count < nInputRequired)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, 1 << 11))
                {
                    GenericCard target = hit.transform.gameObject.GetComponent<GenericCard>();
                    selectedCards.Add(target);

                    target.SetSelected();

                    if (selectedCards.Count == nInputRequired)
                    {
                        AfterInputAction(selectedCards);
                    }
                }
            }
            yield return null;
        }
    }

    public abstract void OnPlayAction();
    public abstract bool MeetsAdditionalPlayRequirements();
    public abstract void AfterInputAction(List<GenericCard> input);
}
