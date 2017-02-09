using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class SwitchingSpell : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return this.Player.InPlay.Items.Concat(this.Player.OppositePlayer.InPlay.Items).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var item = targets.Single();

            var newItem = this.Player.Deck.Items.Skip(Random.Range(0, this.Player.Deck.Items.Count)).First();

            item.Player.Discard.Add(item);

            this.Player.Hand.Add(newItem);

            this.Player.Deck.Shuffle();
        }
    }
}