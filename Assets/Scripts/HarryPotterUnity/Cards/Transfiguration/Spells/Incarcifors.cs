using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Incarcifors : BaseSpell
    {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.Creatures;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.First();
            
            Player.OppositePlayer.Discard.Add(target);
        }
    }
}