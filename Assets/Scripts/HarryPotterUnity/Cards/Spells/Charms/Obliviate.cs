using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class Obliviate : GenericSpell {

        protected override void OnClickAction(List<GenericCard> targets)
        {
            base.OnClickAction(null);

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
