using System.Collections.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class Obliviate : BaseSpell {

        protected override void SpellAction(List<BaseCard> targets)
        {
            int handCount = Player.OppositePlayer.Hand.Cards.Count;
            
            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.OppositePlayer.Hand.Cards[i];

                Player.OppositePlayer.Discard.Add(card);
            }
        }
    }
}
