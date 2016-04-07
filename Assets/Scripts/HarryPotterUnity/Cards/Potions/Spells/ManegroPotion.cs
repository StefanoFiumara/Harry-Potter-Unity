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
            int damage = Player.InPlay.LessonsOfType(LessonTypes.Potions).Count();
            var lesson = Player.InPlay.LessonsOfType(LessonTypes.Potions).First();

            Player.Discard.Add(lesson);

            Player.OppositePlayer.TakeDamage(this, damage);
        }
    }
}
