using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class Incarcifors : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return Player.OppositePlayer.InPlay.Creatures;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            
            Player.OppositePlayer.Discard.Add(target);
        }
    }
}