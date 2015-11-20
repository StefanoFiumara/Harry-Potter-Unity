using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Diffindo : BaseSpell
    {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.CardsExceptStartingCharacter;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            Player.OppositePlayer.Discard.Add(target);
        }
    }
}
