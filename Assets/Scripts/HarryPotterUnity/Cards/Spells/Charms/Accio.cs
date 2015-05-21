using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class Accio : GenericSpell {

        protected override void SpellAction(List<GenericCard> targets)
        {
            //TODO: Implement it with input instead?
            var lessons = Player.Discard.GetCardsOfType(card => card.CardType == CardTypes.Lesson, 2).ToList();
            foreach (var lesson in lessons)
            {
                Player.Hand.Add(lesson);
                Player.Discard.Remove(lesson);
            }

            Player.Hand.AddAll(lessons);
            Player.Discard.RemoveAll(lessons);
        }
    }
}
