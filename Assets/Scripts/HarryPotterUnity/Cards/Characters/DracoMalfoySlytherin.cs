using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Characters
{
    public class DracoMalfoySlytherin : BaseCharacter
    {
        private bool HasEffectActivated { get; set; }

        public override void OnEnterInPlayAction()
        {
            this.Player.OnCardPlayedEvent += this.AddActionOnItemPlayedEvent;
        }

        private void AddActionOnItemPlayedEvent(BaseCard card, List<BaseCard> targets)
        {
            if (this.HasEffectActivated == false && card.Type == Type.Item)
            {
                this.HasEffectActivated = true;

                this.Player.AddActions(1);
            }
        }

        public override void OnExitInPlayAction()
        {
            this.HasEffectActivated = false;

            this.Player.OnCardPlayedEvent -= this.AddActionOnItemPlayedEvent;
        }

        public override void OnInPlayAfterTurnAction()
        {
            this.HasEffectActivated = false;
        }
    }
}