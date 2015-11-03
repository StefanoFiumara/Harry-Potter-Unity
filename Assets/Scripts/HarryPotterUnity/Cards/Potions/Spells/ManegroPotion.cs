using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    [UsedImplicitly]
    public class ManegroPotion : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            int damage = Player.InPlay.GetAmountOfLessonsOfType(LessonTypes.Potions);
            var lesson = Player.InPlay.GetLessonsOfType(LessonTypes.Potions).First();

            Player.Discard.Add(lesson);
            Player.InPlay.Remove(lesson);

            Player.OppositePlayer.TakeDamage(damage);
        }
    }
}
