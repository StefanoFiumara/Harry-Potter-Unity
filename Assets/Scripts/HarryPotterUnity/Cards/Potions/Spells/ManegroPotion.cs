using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    public class ManegroPotion : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int damage = this.Player.InPlay.LessonsOfType(LessonTypes.Potions).Count();
            var lesson = this.Player.InPlay.LessonsOfType(LessonTypes.Potions).First();

            this.Player.Discard.Add(lesson);

            this.Player.OppositePlayer.TakeDamage(this, damage);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.InPlay.LessonsOfType(LessonTypes.Potions).Any();
        }
    }
}
