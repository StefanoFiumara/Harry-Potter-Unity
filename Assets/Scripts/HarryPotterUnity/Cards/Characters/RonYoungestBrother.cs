using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Characters
{
    public class RonYoungestBrother : BaseCharacter
    {
        public override bool CanPerformInPlayAction()
        {
            return this.Player.Hand.Cards.Count == 0
                && this.Player.IsLocalPlayer
                && this.Player.CanUseActions();
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            for (int i = 0; i < 5; i++)
            {
                this.Player.Deck.DrawCard();
            }

            this.Player.UseActions();
        }
    }
}