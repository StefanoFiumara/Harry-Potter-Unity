using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class DiscardLessonFromPlaySpell : BaseSpell
    {
        [Header("Lesson Discard Settings")]
        [SerializeField, UsedImplicitly] private LessonTypes _lessonType;
        [SerializeField, UsedImplicitly] private int _amount;


        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.InPlay.LessonsOfType(_lessonType).Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var lessonsToDiscard = Player.OppositePlayer.InPlay.LessonsOfType(_lessonType).Take(_amount);

            Player.OppositePlayer.Discard.AddAll(lessonsToDiscard);
        }
    }
}