using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class Accio : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var lessons = this.Player.Discard.Lessons.Take(2);

            this.Player.Hand.AddAll(lessons);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.Discard.Lessons.Count >= 2;
        }
    }
}
