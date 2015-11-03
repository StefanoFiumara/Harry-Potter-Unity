using System.Collections.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class EndlessSandwiches : BaseSpell {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int amountCardsToDraw = 7 - Player.Hand.Cards.Count;
            var cardsToDraw = new List<BaseCard>();

            while (amountCardsToDraw-- > 0)
            {
                cardsToDraw.Add( Player.Deck.TakeTopCard() );
            }

            Player.Hand.AddAll(cardsToDraw);
        }
    }
}
