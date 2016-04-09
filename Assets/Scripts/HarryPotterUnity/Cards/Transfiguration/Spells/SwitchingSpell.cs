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
            return Player.InPlay.Items.Concat(Player.OppositePlayer.InPlay.Items).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var item = targets.Single();

            var newItem = Player.Deck.Items.Skip(Random.Range(0, Player.Deck.Items.Count)).First();

            item.Player.Discard.Add(item);

            Player.Hand.Add(newItem);

            Player.Deck.Shuffle();
        }
    }
}