using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    [UsedImplicitly]
    public class HandLimitRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField, UsedImplicitly]
        private int _limitAmount;

        private GenericCard _cardInfo;

        void Start()
        {
            _cardInfo = GetComponent<GenericCard>();
        }
        public bool MeetsRequirement()
        {
            return _cardInfo.Player.Hand.Cards.Count <= _limitAmount;
        }

        public void OnRequirementMet() { }
    }
}
