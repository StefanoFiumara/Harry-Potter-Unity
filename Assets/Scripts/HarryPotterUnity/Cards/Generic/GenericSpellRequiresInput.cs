using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public abstract class GenericSpellRequiresInput : GenericSpell
    {
        [UsedImplicitly, SerializeField]
        private int _inputRequired;

        /// <summary>
        /// Describe any additional requirements that the card must meet in order to be played.
        /// For example: Diffindo requires the opposite player to have at least 1 card in play, otherwise there will be no target.
        /// </summary>
        /// <returns>A boolean describing whether or not the requirements were met.</returns>
        protected abstract bool MeetsAdditionalInputRequirements();
        /// <summary>
        /// Get a list of in-play cards that can be targeted by this card.
        /// For example: Mice to Snuffboxes returns a list of all creatures in play.
        /// </summary>
        /// <returns>An enumerable list of valid targets for this card</returns>
        protected abstract IEnumerable<GenericCard> GetValidCards();
        /// <summary>
        /// Describe the actions that happen after the player has made a selection.
        /// For example: Diffindo will simply discard the selected card.
        /// </summary>
        /// <param name="input">The list of cards that the player has selected to target with this card</param>
        public abstract void AfterInputAction(List<GenericCard> input);
        protected sealed override void OnPlayAction() { }

        protected sealed override bool MeetsAdditionalPlayRequirements()
        {
            return base.MeetsAdditionalPlayRequirements() &&
                   MeetsAdditionalInputRequirements();
        }

        protected sealed override void ExecuteActionAndDiscard()
        {
            Player.Discard.Add(this);
            BeginWaitForInput();
        }

        private void BeginWaitForInput()
        {
            Player.DisableAllCards();
            Player.OppositePlayer.DisableAllCards();

            var validCards = GetValidCards();

            foreach (var card in validCards)
            {
                card.Enable();
                card.gameObject.layer = UtilManager.ValidChoiceLayer;
            }

            StartCoroutine(WaitForPlayerInput());
        }

        private IEnumerator WaitForPlayerInput()
        {
            if (_inputRequired <= 0) throw new Exception("_inputRequired field is not set or set to a negative value!");

            var selectedCards = new List<GenericCard>();

            while (selectedCards.Count < _inputRequired)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && Player.IsLocalPlayer)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << 11))
                    {
                        var target = hit.transform.gameObject.GetComponent<GenericCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == _inputRequired)
                        {
                            var selectedCardIds = selectedCards.Select(c => c.NetworkId).ToArray();
                            Player.MpGameManager.photonView.RPC("ExecuteInputSpellById", PhotonTargets.All, NetworkId, selectedCardIds);
                        }
                    }
                }
                yield return null;
            }
        }
    }
}
