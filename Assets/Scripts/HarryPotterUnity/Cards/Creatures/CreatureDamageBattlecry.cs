using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Creatures
{
    class CreatureDamageBattlecry : GenericCreature, IPersistentCard
    {
        [UsedImplicitly, SerializeField] 
        private int _battlecryDamage;

        public new void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            Player.OppositePlayer.TakeDamage(_battlecryDamage);
        }
    }
}
