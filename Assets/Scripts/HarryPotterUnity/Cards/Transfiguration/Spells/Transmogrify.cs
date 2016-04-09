using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class Transmogrify : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return Player.InPlay.Creatures;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var creature = targets.Single();

            var newCreature = Player.Deck.Creatures.Skip(Random.Range(0, Player.Deck.Creatures.Count)).First();

            Player.Discard.Add(creature);

            Player.Hand.Add(newCreature);

            Player.Deck.Shuffle();
        }

        
    }
}