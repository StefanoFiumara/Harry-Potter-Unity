using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class EngorgementCharm : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            foreach (var creature in this.Player.InPlay.Creatures.Cast<BaseCreature>())
            {
                creature.Heal(creature.MaxHealth);
            }
        }
    }
}