using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class Accio : BaseSpell {

        protected override void SpellAction(List<BaseCard> targets)
        {
            //TODO: Implement it with input instead?
            var lessons = Player.Discard.GetCards(card => card.Type == Type.Lesson).Take(2).ToList();
           
            Player.Hand.AddAll(lessons);
            Player.Discard.RemoveAll(lessons);
        }
    }
}
