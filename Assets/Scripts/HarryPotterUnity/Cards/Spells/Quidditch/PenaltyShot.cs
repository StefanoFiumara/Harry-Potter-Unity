using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Quidditch
{
    [UsedImplicitly]
    public class PenaltyShot : GenericSpell 
    {
        protected override void OnClickAction(List<GenericCard> targets)
        {
            base.OnClickAction(null);
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
        }
    }
}
