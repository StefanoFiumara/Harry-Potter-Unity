using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.DeckGeneration.Requirements
{
    public class DeckContainsLessonTypeRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [SerializeField, UsedImplicitly] private LessonTypes _type;

        public bool MeetsRequirement(List<BaseCard> currentDeck)
        {
            return currentDeck.Exists(c => c is ILessonProvider && ((ILessonProvider)c).LessonType == this._type);
        }
    }
}