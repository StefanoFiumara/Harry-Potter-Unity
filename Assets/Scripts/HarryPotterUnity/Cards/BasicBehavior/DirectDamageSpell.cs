using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class DirectDamageSpell : BaseSpell, IDamageSpell
    {
        
        [Header("Direct Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            DamageAmount = _damageAmount;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            DamageAmount = _damageAmount;
        }
    }
}
