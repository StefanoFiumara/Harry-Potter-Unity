using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class PetrificusTotalus : GenericSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.Discard.GetCards(c => c.Type == Type.Lesson).Count > 0;
        }

        public override List<GenericCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.GetCreaturesInPlay();
        }

        protected override void SpellAction(List<GenericCard> targets)
        {
            if (targets.Count != 1)
            {
                Debug.LogError("More than 1 input sent to Petrificus Totalus");
                return;
            }

            var target = targets[0];
            var player = target.Player;

            var lesson = player.Discard.GetCards(c => c.Type == Type.Lesson).FirstOrDefault();

            if (lesson == null)
            {
                Debug.LogError("No lesson in target player's discard pile");
                return;
            }

            player.InPlay.Remove(target);
            player.Discard.Add(target);

            player.Discard.RemoveAll(new[] {lesson});
            player.InPlay.Add(lesson);
        }
    }
}
