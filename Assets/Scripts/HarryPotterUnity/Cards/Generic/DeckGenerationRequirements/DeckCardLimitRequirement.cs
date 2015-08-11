using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic.DeckGenerationRequirements
{
    public class DeckCardLimitRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [UsedImplicitly, SerializeField] private int _maximumAmountAllowed;

        private GenericCard _cardInfo;

        [UsedImplicitly]
        private void Start()
        {
            _cardInfo = GetComponent<GenericCard>();
        }

        public bool MeetsRequirement(List<GenericCard> currentDeck)
        {
            return currentDeck.Count(c => c.Equals(_cardInfo)) < _maximumAmountAllowed;
        }
    }
}