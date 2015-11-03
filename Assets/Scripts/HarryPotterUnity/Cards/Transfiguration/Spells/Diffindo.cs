using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Diffindo : BaseSpell
    {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.Cards;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            if (targets.Count != 1)
            {
                Debug.LogError("More than one input sent to Diffindo, this should never happen!");
                return;
            }
            
            Player.OppositePlayer.Discard.Add(targets[0]);
            Player.OppositePlayer.InPlay.Remove(targets[0]);
        }
    }
}
