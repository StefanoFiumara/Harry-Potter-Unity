using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class Obliviate : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            int handCount = Player.OppositePlayer.Hand.Cards.Count;
            
            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.OppositePlayer.Hand.Cards[i];

                Player.OppositePlayer.Hand.Remove(card);
                Player.OppositePlayer.Discard.Add(card);
            }
        }
    }
}
