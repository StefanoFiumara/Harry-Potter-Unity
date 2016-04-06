using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class HandLimitRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField, UsedImplicitly]
        private int _limitAmount;

        private BaseCard _cardInfo;

        [UsedImplicitly]
        void Start()
        {
            _cardInfo = GetComponent<BaseCard>();
        }

        public bool MeetsRequirement()
        {
            return _cardInfo.Player.Hand.Cards.Count <= _limitAmount;
        }

        public void OnRequirementMet() { }
    }
}
