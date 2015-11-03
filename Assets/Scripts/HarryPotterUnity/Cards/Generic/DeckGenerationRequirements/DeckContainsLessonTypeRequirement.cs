using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic.DeckGenerationRequirements
{
    public class DeckContainsLessonTypeRequirement : MonoBehaviour, IDeckGenerationRequirement
    {
        [SerializeField, UsedImplicitly] private LessonTypes _type;

        public bool MeetsRequirement(List<GenericCard> currentDeck)
        {
            return currentDeck.Exists(c => c is ILessonProvider && ((ILessonProvider)c).LessonType == _type);
        }
    }
}