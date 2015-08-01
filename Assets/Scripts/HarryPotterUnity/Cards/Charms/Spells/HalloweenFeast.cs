using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class HalloweenFeast : GenericSpell {
        protected override void SpellAction(List<GenericCard> targets)
        {
            //TODO: Implement it with input instead?
            var creatures = Player.Discard.GetCards(card => card.CardType == CardTypes.Creature).Take(4).ToList();

            Player.Discard.RemoveAll(creatures);
            Player.Hand.AddAll(creatures);
        }
    }
}
