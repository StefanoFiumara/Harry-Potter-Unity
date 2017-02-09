using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class Alchemy : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var lessons = this.Player.Deck.Lessons.Take(2);

            this.Player.Hand.AddAll(lessons);
            this.Player.Deck.Shuffle();
        }
    }
}
