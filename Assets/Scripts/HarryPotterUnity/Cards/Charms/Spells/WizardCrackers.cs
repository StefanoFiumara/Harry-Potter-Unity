using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class WizardCrackers : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            var card = Player.Deck.TakeTopCard();

            if (card.Type == CardType.Lesson)
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
