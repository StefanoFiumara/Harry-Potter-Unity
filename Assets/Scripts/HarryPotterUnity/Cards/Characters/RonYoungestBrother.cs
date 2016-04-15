using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Characters
{
    public class RonYoungestBrother : BaseCharacter
    {
        public override bool CanPerformInPlayAction()
        {
            return Player.Hand.Cards.Count == 0
                && Player.IsLocalPlayer
                && Player.CanUseActions();
        }

        public override void OnSelectedAction(List<BaseCard> targets = null)
        {
            for (int i = 0; i < 5; i++)
            {
                Player.Deck.DrawCard();
            }

            Player.UseActions();
        }
    }
}