using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace HarryPotterUnity.Game
{
    [RequireComponent(typeof(BaseCard))]
    [RequireComponent(typeof(InputRequirement))]
    public class InputGatherer : MonoBehaviour
    {
        private BaseCard _cardInfo;

        private int _inputRequired;

        private static int LayerMask
        {
            get { return 1 << 11; }
        }

        private void Start()
        {
            _cardInfo = GetComponent<BaseCard>();
            var requirement = GetComponent<InputRequirement>();

            if (requirement == null)
            {
                Debug.LogError("No Input Requirement Attached to this card");
                return;
            }

            _inputRequired = requirement.InputRequired;

        }

        public void GatherInput()
        {
            _cardInfo.Player.DisableAllCards();
            _cardInfo.Player.OppositePlayer.DisableAllCards();

            var validCards = _cardInfo.GetValidTargets();

            foreach (var card in validCards)
            {
                card.SetHighlight();
                card.Enable();
                card.gameObject.layer = GameManager.VALID_CHOICE_LAYER;
            }

            StartCoroutine(WaitForPlayerInput());
        }

        private IEnumerator WaitForPlayerInput()
        {
            if (_inputRequired <= 0) throw new Exception("_inputRequired field is not set or set to a negative value!");

            var selectedCards = new List<BaseCard>();

            while (selectedCards.Count < _inputRequired)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && _cardInfo.Player.IsLocalPlayer)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, LayerMask))
                    {
                        //BUG: If the player clicks on a non-card collider (e.g. the Deck Collider) Will this give a null reference?
                        var target = hit.transform.gameObject.GetComponent<BaseCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == _inputRequired)
                        {
                            foreach (var card in selectedCards)
                            {
                                card.RemoveHighlight();
                            }

                            var selectedCardIds = selectedCards.Select(c => c.NetworkId).ToArray();
                            GameManager.Network.RPC("ExecuteInputCardById", PhotonTargets.All, _cardInfo.NetworkId, selectedCardIds);
                        }
                    }
                }
                yield return null;
            }
        }
    }
}
