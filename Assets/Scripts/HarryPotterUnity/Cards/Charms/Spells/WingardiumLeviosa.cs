using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class WingardiumLeviosa : GenericSpell
    {

        protected override void SpellAction(List<GenericCard> targets)
        {
            //TODO: Does not work against gargoyles!
            Player.DamageBuffer = Player.OppositePlayer.DamagePerTurn;
        }
    }
}
