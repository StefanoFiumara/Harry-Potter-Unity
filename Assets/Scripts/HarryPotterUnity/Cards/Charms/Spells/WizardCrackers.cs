using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class WizardCrackers : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var card = this.Player.Deck.TakeTopCard();

            if (card.Type == Type.Lesson)
            {
                this.Player.InPlay.Add(card);
            }
            else
            {
                this.Player.Hand.Add(card);
            }
        }
    }
}
