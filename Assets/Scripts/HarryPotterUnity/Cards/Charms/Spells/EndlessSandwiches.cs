using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class EndlessSandwiches : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int amountCardsToDraw = 7 - this.Player.Hand.Cards.Count;
            var cardsToDraw = new List<BaseCard>();

            while (amountCardsToDraw-- > 0)
            {
                var card = this.Player.Deck.TakeTopCard();
                if (card == null) break;

                cardsToDraw.Add(this.Player.Deck.TakeTopCard() );
            }

            this.Player.Hand.AddAll(cardsToDraw);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.Hand.Cards.Count <= 7;
        }
    }
}
