using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    [RequireComponent(typeof(InputRequirement))]
    public class MidAirCollision : BaseSpell, IDamageSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            Player.Discard.Add(target);
            Player.InPlay.Remove(target);
            
            Player.OppositePlayer.TakeDamage(DamageAmount);
        }

        public override List<BaseCard> GetValidTargets()
        {
            return Player.InPlay.CardsExceptStartingCharacter;
        }

        public int DamageAmount { get { return 10; } }
    }
}