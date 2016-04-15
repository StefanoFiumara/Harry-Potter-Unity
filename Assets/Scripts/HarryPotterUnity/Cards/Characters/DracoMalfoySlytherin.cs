using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Characters
{
    public class DracoMalfoySlytherin : BaseCharacter
    {
        private bool HasEffectActivated { get; set; }

        public override void OnEnterInPlayAction()
        {
            Player.OnCardPlayed += AddActionOnItemPlayed;
        }

        private void AddActionOnItemPlayed(BaseCard card, List<BaseCard> targets)
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

            Player.OnCardPlayed -= AddActionOnItemPlayed;
        }

        public override void OnInPlayAfterTurnAction()
        {
            HasEffectActivated = false;
        }
    }
}