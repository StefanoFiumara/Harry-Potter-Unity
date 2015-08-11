using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public class DirectDamageSpell : GenericSpell {
        
        [Header("Direct Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        protected override void SpellAction(List<GenericCard> targets)
        {
            Player.OppositePlayer.TakeDamage(_damageAmount);
        }
    }
}
