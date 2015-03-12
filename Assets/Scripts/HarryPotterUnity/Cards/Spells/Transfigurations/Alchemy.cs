using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Game;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Transfigurations
{
    [UsedImplicitly]
    public class Alchemy : GenericSpell
    {
        protected override void OnPlayAction()
        {
            var lessons = Player.Deck.GetCardsOfType(CardTypes.Lesson, 2);
            foreach (var lesson in lessons)
            {
                Player.Deck.Remove(lesson);
                Player.Hand.Add(lesson);
            }
            Player.Deck.Shuffle();
        }
    }
}
