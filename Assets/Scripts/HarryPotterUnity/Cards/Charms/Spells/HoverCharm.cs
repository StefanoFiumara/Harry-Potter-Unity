﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class HoverCharm : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return this.Player.OppositePlayer.InPlay.CardsExceptStartingCharacter;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            this.Player.OppositePlayer.Hand.Add(target);
        }
    }
}
