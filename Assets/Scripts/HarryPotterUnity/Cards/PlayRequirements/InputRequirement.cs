using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class InputRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private BaseCard _cardInfo;

        [SerializeField, UsedImplicitly]
        private int _fromHandActionInputRequired;

        [SerializeField, UsedImplicitly]
        private int _inPlayActionInputRequired;

        public int FromHandActionInputRequired { get { return _fromHandActionInputRequired; } }
        public int InPlayActionInputRequired { get { return _inPlayActionInputRequired; } }

        private void Awake()
        {
            _cardInfo = GetComponent<BaseCard>();
            if (GetComponent<InputGatherer>() == null)
            {
                gameObject.AddComponent<InputGatherer>();   
            }
        }

        public bool MeetsRequirement()
        {
            switch (_cardInfo.State)
            {
                case State.InHand:
                    return _cardInfo.GetFromHandActionTargets().Count >= _fromHandActionInputRequired;
                case State.InPlay:
                    return _cardInfo.GetInPlayActionTargets().Count >= _inPlayActionInputRequired;
            }

            return false;

        }

        public void OnRequirementMet() { }
    }
}
