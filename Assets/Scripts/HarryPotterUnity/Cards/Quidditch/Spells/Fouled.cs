using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class Fouled : BaseSpell, IDamageSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);
            Player.OppositePlayer.OnNextTurnStart += () => Player.OppositePlayer.AddActions(-1);
        }

        public int DamageAmount { get { return 4; } }
    }
}