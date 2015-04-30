using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class EndlessSandwiches : GenericSpell {
        protected override void SpellAction(List<GenericCard> targets)
        {
            while (Player.Hand.Cards.Count < 7)
            {
                Player.Deck.DrawCard();
            }
        }
    }
}
