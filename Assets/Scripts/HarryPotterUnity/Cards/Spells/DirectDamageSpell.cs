using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells
{
    public class DirectDamageSpell : GenericSpell {
        
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        protected override void OnClickAction(List<GenericCard> targets)
        {
            base.OnClickAction(null);
            Player.OppositePlayer.TakeDamage(_damageAmount);
        }
    }
}
