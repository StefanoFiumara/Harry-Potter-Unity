using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class Accio : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            //TODO: Implement it with input instead?
            var lessons = Player.Discard.GetCards(card => card.Type == CardType.Lesson).Take(2).ToList();
           
            Player.Hand.AddAll(lessons);
            Player.Discard.RemoveAll(lessons);
        }
    }
}
