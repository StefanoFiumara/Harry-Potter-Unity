using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    class CreatureDamageBattlecry : BaseCreature, IPersistentCard
    {
        [UsedImplicitly, SerializeField, Space(10)]
        private int _battlecryDamage;

        public new void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            Player.OppositePlayer.TakeDamage(_battlecryDamage);
        }
    }
}
