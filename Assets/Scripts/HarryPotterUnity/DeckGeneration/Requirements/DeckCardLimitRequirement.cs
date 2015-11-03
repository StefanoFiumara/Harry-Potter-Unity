using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.DeckGeneration.Requirements
{
    public class DeckCardLimitRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [UsedImplicitly, SerializeField] private int _maximumAmountAllowed;

        private BaseCard _cardInfo;

        [UsedImplicitly]
        private void Start()
        {
            _cardInfo = GetComponent<BaseCard>();
        }

        public bool MeetsRequirement(List<BaseCard> currentDeck)
        {
            return currentDeck.Count(c => c.Equals(_cardInfo)) < _maximumAmountAllowed;
        }
    }
}