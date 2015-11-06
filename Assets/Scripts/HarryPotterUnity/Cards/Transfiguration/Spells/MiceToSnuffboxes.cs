using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class MiceToSnuffboxes : BaseSpell {
        public override List<BaseCard> GetValidTargets()
        {
            var validCards = Player.InPlay.GetCreaturesInPlay();
            validCards.AddRange(Player.OppositePlayer.InPlay.GetCreaturesInPlay());

            return validCards;
        }

        protected override void SpellAction(List<BaseCard> selectedCards)
        {
            //Check if the cards belong to the same player and do and AddAll to prevent animation bugs
            bool localPlayer = selectedCards.First().Player.IsLocalPlayer;
            if (selectedCards.All(c => c.Player.IsLocalPlayer == localPlayer))
            {
                selectedCards.First().Player.Hand.AddAll(selectedCards);
                selectedCards.First().Player.InPlay.RemoveAll(selectedCards);

            }
            else
            {
                foreach (var card in selectedCards)
                {
                    card.Player.Hand.Add(card, preview: false);
                    card.Player.InPlay.Remove(card);
                }
            }
        }
    }
}
