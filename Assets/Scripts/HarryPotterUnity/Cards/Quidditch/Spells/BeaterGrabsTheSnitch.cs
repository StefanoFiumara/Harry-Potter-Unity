using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class BeaterGrabsTheSnitch : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return this.Player.InPlay.Matches.Concat(this.Player.OppositePlayer.InPlay.Matches).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            this.Player.OppositePlayer.Hand.Add(target);
        }
    }
}