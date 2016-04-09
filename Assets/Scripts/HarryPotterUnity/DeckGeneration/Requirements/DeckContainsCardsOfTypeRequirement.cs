using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.DeckGeneration.Requirements
{
    public class DeckContainsCardsOfTypeRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [SerializeField, UsedImplicitly] private Type _cardType;

        public bool MeetsRequirement(List<BaseCard> currentDeck)
        {
            return currentDeck.Any(c => c.Type == _cardType);
        }
    }
}