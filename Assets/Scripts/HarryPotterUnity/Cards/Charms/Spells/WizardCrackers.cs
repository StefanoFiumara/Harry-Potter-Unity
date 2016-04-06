using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class WizardCrackers : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var card = Player.Deck.TakeTopCard();

            if (card.Type == Type.Lesson)
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
