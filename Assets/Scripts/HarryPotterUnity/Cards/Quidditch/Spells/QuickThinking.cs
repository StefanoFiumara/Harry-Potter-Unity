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
            DamageAmount = 3;
        }

        public override List<BaseCard> GetFromHandActionTargets()
        {
            return new List<BaseCard> { Player.Deck.StartingCharacter, Player.OppositePlayer.Deck.StartingCharacter };
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            if (target.Player == Player)
            {
                Player.Deck.DrawCard();
                Player.Deck.DrawCard();
                Player.Deck.DrawCard();
            }
            else
            {
                Player.OppositePlayer.TakeDamage(this, DamageAmount);
            }

            DamageAmount = 3;
        }
    }
}