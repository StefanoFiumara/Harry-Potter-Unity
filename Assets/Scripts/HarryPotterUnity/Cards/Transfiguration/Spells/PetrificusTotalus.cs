using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class PetrificusTotalus : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.
                            Discard.GetCards(c => c.Type == Type.Lesson).Count > 0 && 
                            GetValidTargets().Count > 0;
        }

        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.GetCreaturesInPlay();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            if (targets.Count != 1)
            {
                Debug.LogError("More than 1 input sent to Petrificus Totalus");
                return;
            }

            var target = targets.First();
            var player = target.Player;

            var lesson = player.Discard.GetCards(c => c.Type == Type.Lesson).First();
            
            player.InPlay.Remove(target);
            player.Discard.Add(target);

            player.Discard.Remove(lesson);
            player.InPlay.Add(lesson);
        }
    }
}
