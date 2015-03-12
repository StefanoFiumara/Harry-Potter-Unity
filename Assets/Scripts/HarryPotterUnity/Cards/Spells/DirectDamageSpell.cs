using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells
{
    public class DirectDamageSpell : GenericSpell {
        
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        protected override void OnPlayAction()
        {
            Player.OppositePlayer.TakeDamage(_damageAmount);
        }
    }
}
