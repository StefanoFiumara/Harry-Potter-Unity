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
            //TODO: Need to check whether this needs to check the FromHandActionTargets or the InPlayActionTargets!
            return _cardInfo.GetFromHandActionTargets().Count >= _fromHandActionInputRequired;
        }

        public void OnRequirementMet() { }
    }
}
