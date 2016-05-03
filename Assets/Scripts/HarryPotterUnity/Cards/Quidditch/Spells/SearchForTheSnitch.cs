using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class SearchForTheSnitch : BaseSpell
    {
        //TODO: Test this
        protected override void SpellAction(List<BaseCard> targets)
        {
            var cards =
                Player.Deck.Cards.Where(
                    c =>
                        c.Classification == ClassificationTypes.Quidditch ||
                        c is BaseLesson && ((BaseLesson) c).LessonType == LessonTypes.Quidditch).ToList();

            if (cards.Count > 0)
            {
                var selectedCard = cards.Skip(Random.Range(0, cards.Count)).First();
                Player.Hand.Add(selectedCard);
            }
            
            Player.Deck.Shuffle();
        }
    }
}