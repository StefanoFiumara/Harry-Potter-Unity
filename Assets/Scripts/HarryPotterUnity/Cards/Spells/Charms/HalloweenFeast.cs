using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class HalloweenFeast : GenericSpell {
        protected override void SpellAction(List<GenericCard> targets)
        {
            //TODO: Implement it with input instead?
            var creatures = Player.Discard.GetCardsOfType(card => card.CardType == CardTypes.Creature, 4);

            foreach (var creature in creatures)
            {
                Player.Hand.Add(creature);
                Player.Discard.Remove(creature);
            }
        }
    }
}
