using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic.PlayRequirements
{
    [UsedImplicitly]
    class DiscardPileContainsRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField, UsedImplicitly]
        private GenericCard.CardType _type;

        [SerializeField, UsedImplicitly]
        private int _minimumAmount;

        public bool MeetsRequirement()
        {
            var player = GetComponent<GenericCard>().Player;

            return player.Discard.GetCards(card => card.Type == _type).Count >= _minimumAmount;
        }

        public void OnRequirementMet() { }
    }
}
