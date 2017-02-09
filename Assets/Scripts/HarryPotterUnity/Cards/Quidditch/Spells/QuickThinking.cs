using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class QuickThinking : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            this.DamageAmount = 3;
        }

        public override List<BaseCard> GetFromHandActionTargets()
        {
            return new List<BaseCard> {this.Player.Deck.StartingCharacter, this.Player.OppositePlayer.Deck.StartingCharacter };
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            if (target.Player == this.Player)
            {
                this.Player.Deck.DrawCard();
                this.Player.Deck.DrawCard();
                this.Player.Deck.DrawCard();
            }
            else
            {
                this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);
            }

            this.DamageAmount = 3;
        }
    }
}