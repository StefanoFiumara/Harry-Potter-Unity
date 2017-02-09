using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class MidAirCollision : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            this.DamageAmount = 10;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            target.RemoveHighlight();

            this.Player.Discard.Add(target);

            this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);

            this.DamageAmount = 10;
        }

        public override List<BaseCard> GetFromHandActionTargets()
        {
            return this.Player.InPlay.CardsExceptStartingCharacter;
        }
    }
}