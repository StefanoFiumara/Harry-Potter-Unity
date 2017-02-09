using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class WingardiumLeviosa : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            this.Player.TypeImmunity.Add(Type.Creature);

            this.Player.OnNextTurnStartEvent += () => this.Player.TypeImmunity.Remove(Type.Creature);

        }
    }
}
