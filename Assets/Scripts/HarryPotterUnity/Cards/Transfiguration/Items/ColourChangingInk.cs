using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    [UsedImplicitly]
    public class ColourChangingInk : GenericCard, IPersistentCard
    {
        protected override void OnClickAction(List<GenericCard> targets)
        {
            Player.Hand.Remove(this);
            Player.InPlay.Add(this);
        }
        
        public bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.Hand.Cards.Count > 0;
        }

        public void OnSelectedAction()
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

        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnEnterInPlayAction() { }
        public void OnExitInPlayAction() { }
    }
}