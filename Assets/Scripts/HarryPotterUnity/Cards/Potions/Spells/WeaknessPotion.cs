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
            DamageAmount = 5;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            Player.TypeImmunity.Add(Type.Creature);
            Player.OnNextTurnStart += () => Player.TypeImmunity.Remove(Type.Creature);
        }
    }
}