using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic.PlayRequirements
{
    [UsedImplicitly]
    public class InputRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private GenericCard _cardInfo;

        [SerializeField, UsedImplicitly]
        private int _inputRequired;

        public int InputRequired { get { return _inputRequired; } }

        [UsedImplicitly]
        void Awake()
        {
            _cardInfo = GetComponent<GenericCard>();
            if (GetComponent<InputGatherer>() == null)
            {
                gameObject.AddComponent<InputGatherer>();   
            }
        }

        public bool MeetsRequirement()
        {
            return _cardInfo.GetValidTargets().Count >= _inputRequired;
        }

        public void OnRequirementMet() { }
    }
}
