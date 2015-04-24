using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using LessonType = HarryPotterUnity.Cards.Generic.Lesson.LessonTypes;

namespace HarryPotterUnity.Cards.Spells.Potions
{
    [UsedImplicitly]
    public class ManegroPotion : GenericSpell {

        protected override void OnClickAction(List<GenericCard> targets)
        {
            base.OnClickAction(null);

            int damage = Player.InPlay.GetAmountOfLessonsOfType(LessonType.Potions);
            var lesson = Player.InPlay.GetLessonOfType(LessonType.Potions);

            Player.Discard.Add(lesson);
            Player.InPlay.Remove(lesson);

            Player.OppositePlayer.TakeDamage(damage);
        }
    }
}
