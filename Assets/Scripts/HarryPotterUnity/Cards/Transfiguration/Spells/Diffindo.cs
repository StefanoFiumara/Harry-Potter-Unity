using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Diffindo : GenericSpell
    {
        public override List<GenericCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.Cards;
        }

        protected override void SpellAction(List<GenericCard> targets)
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
