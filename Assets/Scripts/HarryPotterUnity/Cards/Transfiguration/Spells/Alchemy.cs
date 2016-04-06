using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class Alchemy : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var lessons = Player.Deck.Lessons.Take(2);
            
            Player.Hand.AddAll(lessons);
            Player.Deck.Shuffle();
        }
    }
}
