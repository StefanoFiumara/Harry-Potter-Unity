using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    class DiscardPileContainsRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField, UsedImplicitly]
        private Type _type;

        [SerializeField, UsedImplicitly]
        private int _minimumAmount;

        public bool MeetsRequirement()
        {
            var player = this.GetComponent<BaseCard>().Player;

            return player.Discard.Cards.Count(card => card.Type == this._type) >= this._minimumAmount;
        }

        public void OnRequirementMet() { }
    }
}
