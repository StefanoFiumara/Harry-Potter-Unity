using System;
using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Spells.Transfigurations
{
    public class Diffindo : GenericSpell {

        public override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.InPlay.Cards.Count > 0;
        }

        protected override List<GenericCard> GetValidCards()
        {
            return Player.OppositePlayer.InPlay.Cards;
        }
        public override void AfterInputAction(List<GenericCard> selectedCards)
        {
            if (selectedCards.Count == 1)
            {
                selectedCards[0].Enable();

                Player.OppositePlayer.InPlay.Remove(selectedCards[0]);
                Player.OppositePlayer.Discard.Add(selectedCards[0]);
            }
            else
            {
                throw new Exception("More than one input sent to Diffindo, this should never happen!");
            }
        }

        public override void OnPlayAction()
        {
            throw new Exception("OnPlayAction called on Diffindo, this should never happen!");
        }
    }
}
