using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class InputRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private BaseCard _cardInfo;

        [SerializeField, UsedImplicitly]
        private int _inputRequired;

        public int InputRequired { get { return _inputRequired; } }

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
            return _cardInfo.GetValidTargets().Count >= _inputRequired;
        }

        public void OnRequirementMet() { }
    }
}
