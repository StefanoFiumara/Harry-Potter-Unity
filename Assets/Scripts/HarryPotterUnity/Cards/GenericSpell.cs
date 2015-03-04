using System;
using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Cards
{
    public abstract class GenericSpell : GenericCard {

        public Lesson.LessonTypes CostType;
        public int CostAmount;

        public int InputRequired;

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        public override void OnClickAction()
        {
            AnimateAndDiscard();
            Player.Hand.Remove(this);
        }

        public override bool MeetsAdditionalPlayRequirements()
        {
            return Player.AmountLessonsInPlay > CostAmount &&
                   Player.LessonTypesInPlay.Contains(CostType) &&
                   MeetsAdditionalInputRequirements();
        }

        protected void AnimateAndDiscard()
        {
            //TODO: Rotate if it's being played by the opponent
            UtilManager.AddTweenToQueue(this, SpellOffset, 0.5f, 0f, State, false, false);
            Invoke("ExecuteActionAndDiscard", 0.9f);
        }

        protected void ExecuteActionAndDiscard()
        {
            Player.Discard.Add(this);
            if (InputRequired == 0)
            {
                OnPlayAction();
                Player.UseActions(ActionCost); //If the card requires input, the action will be used after the input is selected.
            }
            else
            {
                BeginWaitForInput();
            }
        }

        private void BeginWaitForInput()
        {
            //Move ALL invalid colliders to ignoreraycast layer
            Player.DisableAllCards();
            Player.OppositePlayer.DisableAllCards();

            var validCards = GetValidCards();

            //place valid cards in valid layer
            foreach (var card in validCards)
            {
                card.Enable();
                card.gameObject.layer = UtilManager.ValidChoiceLayer;
            }

            StartCoroutine(WaitForPlayerInput());
        }

        protected IEnumerator WaitForPlayerInput()
        {
            if (InputRequired == 0) throw new Exception("This card does not require input!");

            var selectedCards = new List<GenericCard>();

            while (selectedCards.Count < InputRequired)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << 11))
                    {
                        var target = hit.transform.gameObject.GetComponent<GenericCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == InputRequired)
                        {
                            AfterInputAction(selectedCards);

                            Player.EnableAllCards();
                            Player.OppositePlayer.EnableAllCards();

                            Player.UseActions(ActionCost);
                        }
                    }
                }
                yield return null;
            }
        }

        public virtual bool MeetsAdditionalInputRequirements()
        {
            return true;
        }

        protected virtual List<GenericCard> GetValidCards()
        {
            return null;
        }

        public virtual void OnPlayAction() { }
        public virtual void AfterInputAction(List<GenericCard> input) { }


    }
}