using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Cards
{
    public abstract class GenericSpell : GenericCard {

        public Lesson.LessonTypes CostType;
        public int CostAmount;

        public int InputRequired;

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        public void OnMouseUp()
        {
            if (State != CardStates.InHand) return;

            if (!Player.CanUseAction()) return;

            if (Player.AmountLessonsInPlay < CostAmount || !Player.LessonTypesInPlay.Contains(CostType)) return;

            if (!MeetsAdditionalPlayRequirements()) return;

            AnimateAndDiscard();
            Player.Hand.Remove(this);
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
                Player.UseAction(); //If the card requires input, the action will be used after the input is selected.
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

            List<GenericCard> validCards = GetValidCards();

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

                            Player.UseAction();
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
}