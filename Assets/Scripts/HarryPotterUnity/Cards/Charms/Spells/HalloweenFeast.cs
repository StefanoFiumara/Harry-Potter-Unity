using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class HalloweenFeast : BaseSpell {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var creatures = Player.Discard.Creatures.Take(4).ToList();

            Player.Hand.AddAll(creatures);
        }
    }
}
