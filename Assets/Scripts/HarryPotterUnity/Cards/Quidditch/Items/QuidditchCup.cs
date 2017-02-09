using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class QuidditchCup : BaseItem
    {

        private bool HasEffectActivated { get; set; }

        public override void OnEnterInPlayAction()
        {
            this.Player.OnCardPlayedEvent += this.AddActionOnQuidditchCardPlayedEvent;
        }

        private void AddActionOnQuidditchCardPlayedEvent(BaseCard card, List<BaseCard> targets)
        {
            if (this.HasEffectActivated == false && card.Classification == ClassificationTypes.Quidditch)
            {
                this.HasEffectActivated = true;

                this.Player.AddActions(1);
            }
        }

        public override void OnExitInPlayAction()
        {
            this.HasEffectActivated = false;

            this.Player.OnCardPlayedEvent -= this.AddActionOnQuidditchCardPlayedEvent;
        }

        public override void OnInPlayAfterTurnAction()
        {
            this.HasEffectActivated = false;
        }
    }
}