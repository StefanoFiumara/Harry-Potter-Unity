using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    [UsedImplicitly]
    class DiscardPileContainsRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField, UsedImplicitly]
        private GenericCard.CardTypes _type;

        [SerializeField, UsedImplicitly]
        private int _minimumAmount;

        public bool MeetsRequirement()
        {
            var player = GetComponent<GenericCard>().Player;

            return player.Discard.GetCards(card => card.CardType == _type).Count >= _minimumAmount;
        }

        public void OnRequirementMet() { }
    }
}
