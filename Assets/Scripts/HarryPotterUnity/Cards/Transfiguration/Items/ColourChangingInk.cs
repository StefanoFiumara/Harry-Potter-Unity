using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class ColourChangingInk : BaseItem
    { 
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() 
                && Player.Hand.Cards.Count > 0 
                && Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var cardsInHand = Player.Hand.Cards.ToList();
            int cardCount = cardsInHand.Count;
            
            Player.Deck.AddAll( cardsInHand );

            while (cardCount-- > 0)
            {
                Player.Deck.DrawCard();
            }

            Player.UseActions();
        }
    }
}