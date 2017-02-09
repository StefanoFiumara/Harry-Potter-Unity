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
            this.DamageAmount = 4;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);
            this.Player.OppositePlayer.OnNextTurnStartEvent += () => this.Player.OppositePlayer.AddActions(-1);

            this.DamageAmount = 4;
        }
    }
}