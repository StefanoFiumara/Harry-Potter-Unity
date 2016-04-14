using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class WingardiumLeviosa : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.OnNextTurnStart += () =>
            {
                //TODO: Make player immune to all damage from creature cards, not just whatever amount the enemy currently has
                Player.CreatureDamageBuffer = 
                Player.OppositePlayer.InPlay.Creatures
                    .Cast<BaseCreature>()
                    .Sum(card => card.DamagePerTurn);
            };
            
        }
    }
}
