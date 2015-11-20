using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    public class Smash : BaseSpell 
    {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.InPlay.CardsExceptStartingCharacter.Where(c => c.Type == Type.Item).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            Player.OppositePlayer.Discard.Add(target);
        }
    }
}
