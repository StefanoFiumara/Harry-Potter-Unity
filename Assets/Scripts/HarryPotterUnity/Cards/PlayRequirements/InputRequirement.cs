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

        public int FromHandActionInputRequired { get { return this._fromHandActionInputRequired; } }
        public int InPlayActionInputRequired { get { return this._inPlayActionInputRequired; } }

        private void Awake()
        {
            this._cardInfo = this.GetComponent<BaseCard>();
            if (this.GetComponent<InputGatherer>() == null)
            {
                this.gameObject.AddComponent<InputGatherer>();   
            }
        }

        public bool MeetsRequirement()
        {
            switch (this._cardInfo.State)
            {
                case State.InHand:
                    return this._cardInfo.GetFromHandActionTargets().Count >= this._fromHandActionInputRequired;
                case State.InPlay:
                    return this._cardInfo.GetInPlayActionTargets().Count >= this._inPlayActionInputRequired;
            }

            return false;

        }

        public void OnRequirementMet() { }
    }
}
