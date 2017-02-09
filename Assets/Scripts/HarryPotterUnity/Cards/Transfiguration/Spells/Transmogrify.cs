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
            return this.Player.InPlay.Creatures;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var creature = targets.Single();

            var newCreature = this.Player.Deck.Creatures.Skip(Random.Range(0, this.Player.Deck.Creatures.Count)).First();

            this.Player.Discard.Add(creature);

            this.Player.Hand.Add(newCreature);

            this.Player.Deck.Shuffle();
        }

        
    }
}