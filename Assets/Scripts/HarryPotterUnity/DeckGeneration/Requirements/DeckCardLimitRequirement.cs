using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.DeckGeneration.Requirements
{
    public class DeckCardLimitRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [SerializeField] private int _maximumAmountAllowed;

        private BaseCard _cardInfo;

        public int MaximumAmountAllowed
        {
            private get { return this._maximumAmountAllowed; }
            set { this._maximumAmountAllowed = value; }
        }

        [UsedImplicitly]
        private void Start()
        {
            this._cardInfo = this.GetComponent<BaseCard>();
        }

        public bool MeetsRequirement(List<BaseCard> currentDeck)
        {
            return currentDeck.Count(c => c == this._cardInfo) < this.MaximumAmountAllowed;
        }
    }
}