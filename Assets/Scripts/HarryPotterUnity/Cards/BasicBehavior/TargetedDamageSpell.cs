using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class TargetedDamageSpell : BaseSpell {
        
        [Header("Targeted Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        [UsedImplicitly, SerializeField]
        private bool _canTargetPlayer;

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            if (target is BaseCreature)            
                (target as BaseCreature).TakeDamage(_damageAmount);
            else
                Player.OppositePlayer.TakeDamage(_damageAmount);
        }

        public override List<BaseCard> GetValidTargets()
        {
            var targets = new List<BaseCard>();

            if (_canTargetPlayer) targets.Add(Player.OppositePlayer.Deck.StartingCharacter);

            targets.AddRange( Player.OppositePlayer.InPlay.GetCreaturesInPlay() );

            return targets;
        }
    }
}
