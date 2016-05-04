using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [RequireComponent(typeof(BaseCard))]
    [RequireComponent(typeof(InputRequirement))]
    public class InputGatherer : MonoBehaviour
    {
        private BaseCard _cardInfo;

        private int _fromHandInputRequired;
        private int _inPlayInputRequired;

        private static int LayerMask
        {
            get { return 1 << GameManager.VALID_CHOICE_LAYER; }
        }

        private InputGatherMode GatherMode { get; set; }

        private void Start()
        {
            _cardInfo = GetComponent<BaseCard>();
            var requirement = GetComponent<InputRequirement>();

            if (requirement == null)
            {
                Debug.LogError("No Input Requirement Attached to this card");
                return;
            }

            _fromHandInputRequired = requirement.FromHandActionInputRequired;
            _inPlayInputRequired = requirement.InPlayActionInputRequired;

        }

        public void GatherInput(InputGatherMode mode)
        {
            GatherMode = mode;
            _cardInfo.Player.DisableAllCards();
            _cardInfo.Player.OppositePlayer.DisableAllCards();

            var validCards = GetValidTargets();

            foreach (var card in validCards)
            {
                card.SetHighlight();
                card.Enable();
                card.gameObject.layer = GameManager.VALID_CHOICE_LAYER;
            }

            GameManager.IsInputGathererActive = true;

            StartCoroutine(WaitForPlayerInput());
        }

        private IEnumerator WaitForPlayerInput()
        {
            int inputRequired = GetInputRequired();

            if (inputRequired <= 0) throw new Exception("Input required field is not set or set to a negative value!");

            var selectedCards = new List<BaseCard>();

            while (selectedCards.Count < inputRequired)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0) && _cardInfo.Player.IsLocalPlayer)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, LayerMask))
                    {
                        var target = hit.transform.gameObject.GetComponent<BaseCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == inputRequired)
                        {
                            foreach (var card in selectedCards)
                            {
                                card.RemoveHighlight();
                            }

                            ExecuteAction(selectedCards);
                        }
                    }
                }
                yield return null;
            }
        }

        private int GetInputRequired()
        {
            switch (GatherMode)
            {
                case InputGatherMode.FromHandAction:
                    return _fromHandInputRequired;
                case InputGatherMode.InPlayAction:
                    return _inPlayInputRequired;
            }
            return 0;
        }

        private List<BaseCard> GetValidTargets()
        {
            switch (GatherMode)
            {
                case InputGatherMode.FromHandAction:
                    return _cardInfo.GetFromHandActionTargets();
                case InputGatherMode.InPlayAction:
                    return _cardInfo.GetInPlayActionTargets();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecuteAction(List<BaseCard> selectedCards)
        {
            var selectedCardIds = selectedCards.Select(c => c.NetworkId).ToArray();

            switch (GatherMode)
            {
                case InputGatherMode.FromHandAction:
                    GameManager.Network.RPC("ExecuteInputCardById", PhotonTargets.All, _cardInfo.NetworkId, selectedCardIds);
                    break;
                case InputGatherMode.InPlayAction:
                    GameManager.Network.RPC("ExecuteInPlayInputCardById", PhotonTargets.All, _cardInfo.NetworkId, selectedCardIds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameManager.IsInputGathererActive = false;
        }
    }
}
