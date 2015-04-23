using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Transfigurations
{
    [UsedImplicitly]
    public class MiceToSnuffboxes : GenericSpellRequiresInput {
        public override List<GenericCard> GetValidCards()
        {
            var validCards = Player.InPlay.GetCreaturesInPlay();
            validCards.AddRange(Player.OppositePlayer.InPlay.GetCreaturesInPlay());

            return validCards;
        }

        public override void AfterInputAction(List<GenericCard> selectedCards)
        {
            foreach(var card in selectedCards) {
                card.Player.Hand.Add(card, false);
                card.Player.InPlay.Remove(card);
            }
        }
    }
}
