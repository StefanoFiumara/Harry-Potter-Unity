using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class PenaltyShot : BaseSpell 
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
        }
    }
}
