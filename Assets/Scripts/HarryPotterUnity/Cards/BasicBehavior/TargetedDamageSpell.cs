using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    [RequireComponent(typeof(InputRequirement))]
    public class TargetedDamageSpell : BaseSpell, IDamageSpell
    {
        [Header("Targeted Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        [UsedImplicitly, SerializeField]
        private bool _canTargetPlayer;

        public int DamageAmount { get { return _damageAmount; } }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            if (target is BaseCreature)
            {
                (target as BaseCreature).TakeDamage(_damageAmount);
            }
            else
            {
                Player.OppositePlayer.TakeDamage(this, _damageAmount);
            }
                
        }

        public override List<BaseCard> GetValidTargets()
        {
            var targets = new List<BaseCard>();

            if (_canTargetPlayer) targets.Add(Player.OppositePlayer.Deck.StartingCharacter);

            targets.AddRange(Player.OppositePlayer.InPlay.Creatures);

            return targets;
        }
    }
}
