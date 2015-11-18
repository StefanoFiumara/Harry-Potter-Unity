using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Alchemy : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var lessons = Player.Deck.GetCardsOfType(Type.Lesson, 2).ToList();
            
            Player.Hand.AddAll(lessons);
            Player.Deck.Shuffle();
        }
    }
}
