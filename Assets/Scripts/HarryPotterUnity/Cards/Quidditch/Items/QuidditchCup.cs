using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class QuidditchCup : BaseItem
    {

        private bool HasEffectActivated { get; set; }

        public override void OnEnterInPlayAction()
        {
            Player.OnCardPlayed += AddActionOnQuidditchCardPlayed;
        }

        private void AddActionOnQuidditchCardPlayed(BaseCard card, List<BaseCard> targets)
        {
            if (HasEffectActivated == false && card.Classification == ClassificationTypes.Quidditch)
            {
                HasEffectActivated = true;

                Player.AddActions(1);
            }
        }

        public override void OnExitInPlayAction()
        {
            HasEffectActivated = false;

            Player.OnCardPlayed -= AddActionOnQuidditchCardPlayed;
        }

        public override void OnInPlayAfterTurnAction()
        {
            HasEffectActivated = false;
        }
    }
}