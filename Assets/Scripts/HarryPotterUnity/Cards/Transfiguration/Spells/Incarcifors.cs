using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Incarcifors : BaseSpell
    {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.GetCreaturesInPlay();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            if (targets.Count != 1)
            {
                Debug.LogError("More or less than one input sent to Incarcifors");
                return;
            }

            var target = targets[0];
            
            Player.OppositePlayer.Discard.Add(target);
            Player.OppositePlayer.InPlay.Remove(target);
        }
    }
}