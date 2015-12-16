using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Game;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class WingardiumLeviosa : BaseSpell
    {

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.OnTurnStartEvent += () =>
            {
                Player.CreatureDamageBuffer = Player.OppositePlayer.InPlay.Cards.OfType<BaseCreature>().Sum(card => card.DamagePerTurn);
            };
            
        }
    }
}
