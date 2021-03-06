﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public static class Extensions
    {
        public static IEnumerable<BaseCard> LessonsOfType(this CardCollection collection, LessonTypes type)
        {
            return collection.Lessons.Where(c => ((BaseLesson)c).LessonType == type);
        }

        public static bool IsQuidditchDamageCard(this BaseCard card)
        {
            return card is IDamageSpell &&
                   card.Classification == ClassificationTypes.Quidditch;
        }

        public static Color WithAlpha(this Color color, float target)
        {
            color.a = target;
            return color;
        }
    }
}