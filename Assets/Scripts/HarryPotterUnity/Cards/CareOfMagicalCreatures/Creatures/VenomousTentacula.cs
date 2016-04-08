using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class VenomousTentacula : BaseCreature
    {
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.Discard.LessonsOfType(LessonTypes.Potions).Any();
        }

        public override void OnSelectedAction(List<BaseCard> targets = null)
        {
            Player.Discard.Add(this);

            var lesson = Player.Discard.LessonsOfType(LessonTypes.Potions).First();

            Player.InPlay.Add(lesson);

            Player.UseActions();
        }
    }
}