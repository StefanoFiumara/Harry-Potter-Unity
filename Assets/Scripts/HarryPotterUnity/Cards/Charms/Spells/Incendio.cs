using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class Incendio : TargetedDamageSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            DamageAmount = Player.InPlay.LessonsOfType(LessonTypes.Charms).Count();

            base.SpellAction(targets);
        }

    }
}