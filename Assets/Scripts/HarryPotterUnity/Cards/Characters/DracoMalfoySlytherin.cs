using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Characters
{
    public class DracoMalfoySlytherin : BaseCharacter
    {
        private bool HasEffectActivated { get; set; }

        public override void OnEnterInPlayAction()
        {
            Player.OnCardPlayedEvent += AddActionOnItemPlayedEvent;
        }

        private void AddActionOnItemPlayedEvent(BaseCard card, List<BaseCard> targets)
        {
            if (HasEffectActivated == false && card.Type == Type.Item)
            {
                HasEffectActivated = true;

                Player.AddActions(1);
            }
        }

        public override void OnExitInPlayAction()
        {
            HasEffectActivated = false;

            Player.OnCardPlayedEvent -= AddActionOnItemPlayedEvent;
        }

        public override void OnInPlayAfterTurnAction()
        {
            HasEffectActivated = false;
        }
    }
}