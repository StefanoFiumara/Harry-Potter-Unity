using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class WizardCrackers : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            var card = Player.Deck.TakeTopCard();

            if (card.CardType == CardTypes.Lesson)
            {
                Player.InPlay.Add(card);
            }
            else
            {
                Player.Hand.Add(card);
            }
        }
    }
}
