using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Transfigurations
{
    [UsedImplicitly]
    public class Diffindo : GenericSpellRequiresInput {
        protected override bool MeetsAdditionalInputRequirements()
        {
            return Player.OppositePlayer.InPlay.Cards.Count > 0;
        }

        protected override IEnumerable<GenericCard> GetValidCards()
        {
            return Player.OppositePlayer.InPlay.Cards;
        }

        public override void AfterInputAction(List<GenericCard> selectedCards)
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
