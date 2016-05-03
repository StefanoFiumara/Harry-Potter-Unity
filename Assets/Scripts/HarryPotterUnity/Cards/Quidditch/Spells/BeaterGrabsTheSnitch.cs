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
            return Player.InPlay.Matches.Concat(Player.OppositePlayer.InPlay.Matches).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            Player.OppositePlayer.Hand.Add(target);
        }
    }
}