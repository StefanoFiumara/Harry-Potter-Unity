using System.Collections.Generic;
using HarryPotterUnity.Cards;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.DeckGeneration.Requirements
{
    [UsedImplicitly]
    public class DeckDoesNotContainCardWithNameRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [SerializeField, UsedImplicitly]
        private string _cardName;

        public bool MeetsRequirement(List<BaseCard> currentDeck)
        {
            return !currentDeck.Exists(card => card.CardName.Contains(this._cardName));
        }
    }
}
