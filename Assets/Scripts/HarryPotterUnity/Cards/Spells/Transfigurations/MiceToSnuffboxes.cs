using System.Collections.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Transfigurations
{
    [UsedImplicitly]
    public class MiceToSnuffboxes : GenericSpell {

        protected override IEnumerable<GenericCard> GetValidCards()
        {
            var validCards = Player.InPlay.GetCreaturesInPlay();
            validCards.AddRange(Player.OppositePlayer.InPlay.GetCreaturesInPlay());

            return validCards;
        }

        protected override bool MeetsAdditionalInputRequirements()
        {
            //There must be at least 2 creatures in play
            var validCards = Player.InPlay.GetCreaturesInPlay();
            validCards.AddRange(Player.OppositePlayer.InPlay.GetCreaturesInPlay());

            return validCards.Count >= 2;
        }

        public override void AfterInputAction(List<GenericCard> selectedCards)
        {
            foreach(var card in selectedCards) {
                card.Player.InPlay.Remove(card);
                card.Player.Hand.Add(card, false);
            }
        }
    }
}
