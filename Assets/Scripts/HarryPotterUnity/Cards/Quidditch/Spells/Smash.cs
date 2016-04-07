using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class Smash : BaseSpell 
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return Player.OppositePlayer.InPlay.Items;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            Player.OppositePlayer.Discard.Add(target);
        }
    }
}
