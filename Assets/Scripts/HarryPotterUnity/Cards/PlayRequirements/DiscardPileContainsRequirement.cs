using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    [UsedImplicitly]
    class DiscardPileContainsRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField, UsedImplicitly]
        private Type _type;

        [SerializeField, UsedImplicitly]
        private int _minimumAmount;

        public bool MeetsRequirement()
        {
            var player = GetComponent<BaseCard>().Player;

            return player.Discard.Cards.Count(card => card.Type == _type) >= _minimumAmount;
        }

        public void OnRequirementMet() { }
    }
}
