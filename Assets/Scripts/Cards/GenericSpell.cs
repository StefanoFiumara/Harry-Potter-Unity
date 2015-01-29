using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LessonTypes = Assets.Scripts.Cards.Lesson.LessonTypes;

namespace Assets.Scripts.Cards
{
    public abstract class GenericSpell : GenericCard {

        public LessonTypes CostType;
        public int CostAmount;

        public int InputRequired;

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        public void OnMouseUp()
        {
            if (State != CardStates.InHand) return;

            if (!_Player.CanUseAction()) return;

            if (_Player.AmountLessonsInPlay < CostAmount || !_Player.LessonTypesInPlay.Contains(CostType)) return;

            if (!MeetsAdditionalPlayRequirements()) return;

            AnimateAndDiscard();
            _Player._Hand.Remove(this);
        }

        protected void AnimateAndDiscard()
        {
            //TODO: Rotate if it's being played by the opponent
            Helper.AddTweenToQueue(this, SpellOffset, 0.5f, 0f, State, false, false);
            Invoke("ExecuteActionAndDiscard", 0.9f);
        }

        protected void ExecuteActionAndDiscard()
        {
            _Player._Discard.Add(this);
            if (InputRequired == 0)
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
                card.gameObject.layer = Helper.ValidChoiceLayer;
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
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << 11))
                    {
                        GenericCard target = hit.transform.gameObject.GetComponent<GenericCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == InputRequired)
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
}