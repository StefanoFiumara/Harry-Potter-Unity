using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class FlooPowder : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var location = Player.Deck.Locations.Skip(Player.Deck.Locations.Count).FirstOrDefault();

            if (location != null)
            {
                Player.Hand.Add(location);
                Player.Deck.Shuffle();
            }
        }
    }
}