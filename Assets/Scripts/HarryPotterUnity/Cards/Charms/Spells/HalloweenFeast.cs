using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class HalloweenFeast : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var creatures = Player.Discard.Creatures.Take(4).ToList();

            Player.Hand.AddAll(creatures);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.Discard.Creatures.Count >= 1;
        }
    }
}
