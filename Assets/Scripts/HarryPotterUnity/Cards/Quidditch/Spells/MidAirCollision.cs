using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    public class MidAirCollision : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            Player.Discard.Add(target);
            Player.InPlay.Remove(target);
            
            Player.OppositePlayer.TakeDamage(10);
        }

        public override List<BaseCard> GetValidTargets()
        {
            return Player.InPlay.CardsExceptStartingCharacter;
        }
    }
}