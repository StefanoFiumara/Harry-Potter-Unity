using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class DirectDamageSpell : BaseSpell, IDamageSpell {
        
        [Header("Direct Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        public int DamageAmount { get { return _damageAmount; } }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, _damageAmount);
        }
    }
}
