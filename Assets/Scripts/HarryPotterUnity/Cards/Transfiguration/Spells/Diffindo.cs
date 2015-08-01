﻿using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Diffindo : GenericSpell {
        public override List<GenericCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.Cards;
        }

        protected override void SpellAction(List<GenericCard> selectedCards)
        {
            if (selectedCards.Count == 1)
            {
                selectedCards[0].Enable();

                Player.OppositePlayer.Discard.Add(selectedCards[0]);
                Player.OppositePlayer.InPlay.Remove(selectedCards[0]);
            }
            else
            {
                throw new Exception("More than one input sent to Diffindo, this should never happen!");
            }
        }
    }
}