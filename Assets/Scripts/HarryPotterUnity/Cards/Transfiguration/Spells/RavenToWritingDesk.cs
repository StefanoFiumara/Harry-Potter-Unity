using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class RavenToWritingDesk : GenericSpell
    {
        public override List<GenericCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.GetCreaturesInPlay();
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count >= 2;
        }

        protected override void SpellAction(List<GenericCard> targets)
        {
            if (targets.Count != 1)
            {
                Debug.LogError("Wrong Input sent to RavenToWritingDesk");
                return;
            }

            Player.OppositePlayer.Discard.Add(targets[0]);
            Player.OppositePlayer.InPlay.Remove(targets[0]);
            
        }
    }
}
