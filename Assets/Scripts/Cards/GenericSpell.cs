using UnityEngine;
using System;
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

        if (_Player.CanUseAction())
        {
            if (_Player.nLessonsInPlay >= CostAmount && _Player.LessonTypesInPlay.Contains(CostType))
            {
                if (MeetsAdditionalPlayRequirements())
                {
                    AnimateAndDiscard();
                    _Player._Hand.Remove(this);
                }
            }
        }
    }

    protected void AnimateAndDiscard()
    {
        //TODO: Rotate if it's being played by the opponent
        Helper.AddTweenToQueue(this, SPELL_OFFSET, 0.5f, 0f, State, false, false);
        Invoke("ExecuteActionAndDiscard", 0.9f);
    }

    protected void ExecuteActionAndDiscard()
    {
        _Player._Discard.Add(this);
        if (nInputRequired == 0)
        {
            OnPlayAction();
            _Player.UseAction(); //If the card requires input, the action will be used after the input is selected.
        }
        else
        {
            BeginWaitForInput();
        }
    }

    private void BeginWaitForInput()
    {
        //Move ALL invalid colliders to ignoreraycast layer
        _Player.DisableAllCards();
        _Player._OppositePlayer.DisableAllCards();

        List<GenericCard> validCards = GetValidCards();
        //place valid cards in valid layer
        foreach (var card in validCards)
        {
            card.Enable();
            card.gameObject.layer = Helper.VALID_CHOICE_LAYER;
        }

        var SelectedCards = new List<GenericCard>();
        StartCoroutine(WaitForPlayerInput(SelectedCards));
    }

    protected IEnumerator WaitForPlayerInput(List<GenericCard> selectedCards)
    {
        if (nInputRequired == 0) throw new Exception("This card does not require input!");

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

                        _Player.EnableAllCards();
                        _Player._OppositePlayer.EnableAllCards();

                        _Player.UseAction();
                    }
                }
            }
            yield return null;
        }
    }

    public abstract void OnPlayAction();
    public abstract bool MeetsAdditionalPlayRequirements();
    public abstract void AfterInputAction(List<GenericCard> input);
    protected abstract List<GenericCard> GetValidCards();
}