using System.Collections.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    public class Fouled : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(4);
            Player.OppositePlayer.OnTurnStartEvent += () => Player.OppositePlayer.AddActions(-1);
        }
    }
}