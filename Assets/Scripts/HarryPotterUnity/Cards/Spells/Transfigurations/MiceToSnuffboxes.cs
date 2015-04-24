using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Transfigurations
{
    [UsedImplicitly]
    public class MiceToSnuffboxes : GenericSpell {
        public override List<GenericCard> GetValidTargets()
        {
            var validCards = Player.InPlay.GetCreaturesInPlay();
            validCards.AddRange(Player.OppositePlayer.InPlay.GetCreaturesInPlay());

            return validCards;
        }

        protected override void OnClickAction(List<GenericCard> selectedCards)
        {
            base.OnClickAction(null);
            foreach(var card in selectedCards) {
                card.Player.Hand.Add(card, false);
                card.Player.InPlay.Remove(card);
            }
        }
    }
}
