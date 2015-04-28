using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Charms
{
    [UsedImplicitly]
    public class WingardiumLeviosa : GenericSpell
    {

        protected override void OnClickAction(List<GenericCard> targets)
        {
            base.OnClickAction(null);

            //TODO: Does not work against gargoyles!
            Player.DamageBuffer = Player.OppositePlayer.DamagePerTurn;
        }
    }
}
