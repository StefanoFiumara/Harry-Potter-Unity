using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class PetrificusTotalus : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.OppositePlayer.
                Discard.Cards.Any(c => c.Type == Type.Lesson);
        }

        public override List<BaseCard> GetFromHandActionTargets()
        {
            return this.Player.OppositePlayer.InPlay.Creatures;
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

            var lesson = player.Discard.Cards.First(c => c.Type == Type.Lesson);
            
            player.Discard.Add(target);
            
            player.InPlay.Add(lesson);
        }
    }
}
