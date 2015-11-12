using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class HoverCharm : BaseSpell {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.CardsExceptStartingCharacter;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.First();

            Player.OppositePlayer.Hand.Add(target);
            Player.OppositePlayer.InPlay.Remove(target);
        }
    }
}
