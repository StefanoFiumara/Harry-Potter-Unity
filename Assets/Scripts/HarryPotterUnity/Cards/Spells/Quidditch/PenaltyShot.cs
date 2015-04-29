using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Quidditch
{
    [UsedImplicitly]
    public class PenaltyShot : GenericSpell 
    {
        protected override void SpellAction(List<GenericCard> targets)
        {
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
        }
    }
}
