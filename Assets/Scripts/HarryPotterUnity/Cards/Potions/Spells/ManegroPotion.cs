using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    [UsedImplicitly]
    public class ManegroPotion : BaseSpell {

        protected override void SpellAction(List<BaseCard> targets)
        {
            int damage = Player.InPlay.GetAmountOfLessonsOfType(LessonTypes.Potions);
            var lesson = Player.InPlay.GetLessonsOfType(LessonTypes.Potions).First();

            Player.Discard.Add(lesson);
            Player.InPlay.Remove(lesson);

            Player.OppositePlayer.TakeDamage(damage);
        }
    }
}
