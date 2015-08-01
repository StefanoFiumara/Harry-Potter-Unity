using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class HoverCharm : GenericSpell {
        public override List<GenericCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.Cards;
        }

        protected override void SpellAction(List<GenericCard> targets)
        {
            var target = targets.First();

            Player.OppositePlayer.Hand.Add(target);
            Player.OppositePlayer.InPlay.Remove(target);
        }
    }
}
