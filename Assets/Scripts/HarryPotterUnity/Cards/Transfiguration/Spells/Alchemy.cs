using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Alchemy : GenericSpell
    {
        protected override void SpellAction(List<GenericCard> targets)
        {
            var lessons = Player.Deck.GetCardsOfType(CardType.Lesson, 2).ToList();
            foreach (var lesson in lessons)
            {
                Player.Deck.Remove(lesson);
            }

            Player.Hand.AddAll(lessons);
            Player.Deck.Shuffle();
        }
    }
}
