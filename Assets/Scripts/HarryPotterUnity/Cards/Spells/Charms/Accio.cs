using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class Accio : GenericSpell {

        protected override void OnPlayAction()
        {
            //TODO: Implement it with input instead?
            var lessons = Player.Discard.GetCardsOfType(card => card.CardType == CardTypes.Lesson, 2);
            foreach (var lesson in lessons)
            {
                Player.Discard.Remove(lesson);
                Player.Hand.Add(lesson);
            }

        }
    }
}
