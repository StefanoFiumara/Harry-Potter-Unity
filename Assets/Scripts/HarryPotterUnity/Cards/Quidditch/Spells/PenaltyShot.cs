using System.Collections.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
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
