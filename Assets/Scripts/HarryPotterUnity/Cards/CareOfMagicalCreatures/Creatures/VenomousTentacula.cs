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
            return this.Player.CanUseActions() 
                && this.Player.IsLocalPlayer
                && this.Player.Discard.LessonsOfType(LessonTypes.Potions).Any();
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            this.Player.Discard.Add(this);

            var lesson = this.Player.Discard.LessonsOfType(LessonTypes.Potions).First();

            this.Player.InPlay.Add(lesson);

            this.Player.UseActions();
        }
    }
}