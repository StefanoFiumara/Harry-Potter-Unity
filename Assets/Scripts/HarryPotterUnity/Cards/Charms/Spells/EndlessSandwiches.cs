using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class EndlessSandwiches : GenericSpell {
        protected override void SpellAction(List<GenericCard> targets)
        {
            int amountCardsToDraw = 7 - Player.Hand.Cards.Count;
            var cardsToDraw = new List<GenericCard>();

            while (amountCardsToDraw-- > 0)
            {
                cardsToDraw.Add( Player.Deck.TakeTopCard() );
            }

            Player.Hand.AddAll(cardsToDraw);
        }
    }
}
