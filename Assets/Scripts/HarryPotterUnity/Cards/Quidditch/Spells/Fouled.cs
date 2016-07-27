using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class Fouled : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            DamageAmount = 4;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);
            Player.OppositePlayer.OnNextTurnStartEvent += () => Player.OppositePlayer.AddActions(-1);

            DamageAmount = 4;
        }
    }
}