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
            //TODO: Implement it with input instead?
            var creatures = Player.Discard.GetCards(card => card.Type == Type.Creature).Take(4).ToList();

            Player.Discard.RemoveAll(creatures);
            Player.Hand.AddAll(creatures);
        }
    }
}
