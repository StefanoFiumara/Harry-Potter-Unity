using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    [UsedImplicitly]
    public class ColourChangingInk : BaseCard, IPersistentCard
    {
        protected override void OnClickAction(List<BaseCard> targets)
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