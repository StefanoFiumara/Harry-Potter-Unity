using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards
{
    public class DirectDamageSpell : BaseSpell {
        
        [Header("Direct Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(_damageAmount);
        }
    }
}
