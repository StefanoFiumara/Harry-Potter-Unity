using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.DeckGenerationRequirements
{
    public class DeckContainsCardWithNameRequirement : MonoBehaviour, IDeckGenerationRequirement
    {

        [SerializeField, UsedImplicitly] private string _cardName;

        public bool MeetsRequirement(List<BaseCard> currentDeck)
        {
            return currentDeck.Exists(card => card.CardName.Contains(_cardName));
        }
    }
}
