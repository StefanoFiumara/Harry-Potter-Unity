using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using LessonType = HarryPotterUnity.Cards.Generic.Lesson.LessonTypes;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    [UsedImplicitly]
    public class ManegroPotion : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            int damage = Player.InPlay.GetAmountOfLessonsOfType(LessonType.Potions);
            var lesson = Player.InPlay.GetLessonsOfType(LessonType.Potions).First();

            Player.Discard.Add(lesson);
            Player.InPlay.Remove(lesson);

            Player.OppositePlayer.TakeDamage(damage);
        }
    }
}
