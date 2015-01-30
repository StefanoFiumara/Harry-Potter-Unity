using System.Collections.Generic;

namespace Assets.Scripts.HarryPotterUnity.Cards.Spells.Transfigurations
{
    public class MiceToSnuffboxes : GenericSpell {

        protected override List<GenericCard> GetValidCards()
        {
            var validCards = Player.InPlay.GetCreaturesInPlay();
            validCards.AddRange(Player.OppositePlayer.InPlay.GetCreaturesInPlay());

            return validCards;
        }
        public override bool MeetsAdditionalPlayRequirements()
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
                card.Player.Hand.Add(card, false, false);
            }
        }

        public override void OnPlayAction() { }
    }
}
