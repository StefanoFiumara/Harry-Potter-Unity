using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Potions
{
    [UsedImplicitly]
    class BoilCure : GenericSpell
    {
        protected override void OnClickAction(List<GenericCard> targets)
        {
            base.OnClickAction(null);
            var cards = Player.Discard.GetCardsOfType(card => !card.Tags.Contains(Tag.Healing), 4);

            foreach (var card in cards)
            {
                Player.Discard.Remove(card);
                Player.Deck.Add(card);
            }
        }
    }
}
