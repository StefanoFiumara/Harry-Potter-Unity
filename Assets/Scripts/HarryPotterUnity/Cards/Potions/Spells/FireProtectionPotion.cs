using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    public class FireProtectionPotion : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            DamageAmount = 3;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            Player.TypeImmunity.Add(Type.Spell);
            Player.OnNextTurnStartEvent += () => Player.TypeImmunity.Remove(Type.Spell);
        }
    }
}