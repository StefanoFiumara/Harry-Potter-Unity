using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic.DeckGenerationRequirements
{
    public class DeckContainsCardWithNameRequirement : MonoBehaviour, IDeckGenerationRequirement
    {

        [SerializeField, UsedImplicitly] private string _cardName;

        public bool MeetsRequirement(List<GenericCard> currentDeck)
        {
            return currentDeck.Exists(card => card.CardName.Contains(_cardName));
        }
    }
}
