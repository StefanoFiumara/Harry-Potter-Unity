using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class HalloweenFeast : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var creatures = this.Player.Discard.Creatures.Take(4).ToList();

            this.Player.Hand.AddAll(creatures);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.Discard.Creatures.Count >= 1;
        }
    }
}
