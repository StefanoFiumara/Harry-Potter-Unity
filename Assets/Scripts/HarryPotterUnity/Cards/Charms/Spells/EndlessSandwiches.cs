using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class EndlessSandwiches : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int amountCardsToDraw = 7 - Player.Hand.Cards.Count;
            var cardsToDraw = new List<BaseCard>();

            while (amountCardsToDraw-- > 0)
            {
                var card = Player.Deck.TakeTopCard();
                if (card == null) break;

                cardsToDraw.Add( Player.Deck.TakeTopCard() );
            }

            Player.Hand.AddAll(cardsToDraw);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.Hand.Cards.Count <= 7;
        }
    }
}
