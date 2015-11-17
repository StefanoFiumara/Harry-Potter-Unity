using System.Linq;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    [UsedImplicitly]
    public class ColourChangingInk : BaseItem
    { 
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.Hand.Cards.Count > 0;
        }

        public override void OnSelectedAction()
        {
            var cardsInHand = Player.Hand.Cards.ToList();
            int cardCount = cardsInHand.Count;

            Player.Hand.RemoveAll( cardsInHand );
            Player.Deck.AddAll( cardsInHand );

            while (cardCount-- > 0)
            {
                Player.Deck.DrawCard();
            }

            Player.UseActions();
        }
    }
}