using System;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    [UsedImplicitly]
    public class InputRequirement : MonoBehaviour, ICardPlayRequirement
    {

        private GenericSpellRequiresInput _cardInfo;

        [UsedImplicitly]
        void Start()
        {
            _cardInfo = GetComponent<GenericSpellRequiresInput>();
        }

        public bool MeetsRequirement()
        {
            return _cardInfo.GetValidCards().Count >= _cardInfo.InputRequired;
        }

        public void OnRequirementMet() { }
    }
}
