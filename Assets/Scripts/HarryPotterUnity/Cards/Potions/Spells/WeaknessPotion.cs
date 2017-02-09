using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    public class WeaknessPotion : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            this.DamageAmount = 5;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);

            this.Player.TypeImmunity.Add(Type.Creature);
            this.Player.OnNextTurnStartEvent += () => this.Player.TypeImmunity.Remove(Type.Creature);
        }
    }
}