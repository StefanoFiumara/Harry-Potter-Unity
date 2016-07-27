using System;
using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Locations
{
    public class GreenhouseThree : BaseLocation
    {
        private bool HasEffectActivated { get; set; }

        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            Player.OnCardPlayedEvent += AddActionOnPlantPlayedEvent;
            Player.OppositePlayer.OnCardPlayedEvent += AddActionOnPlantPlayedEvent;
        }

        private void AddActionOnPlantPlayedEvent(BaseCard card, List<BaseCard> targets)
        {
            if (HasEffectActivated == false && card.HasTag(Tag.Plant))
            {
                HasEffectActivated = true;

                var player = Player.CanUseActions() ? Player : Player.OppositePlayer;

                player.AddActions(1);

                player.OppositePlayer.OnNextTurnStartEvent += () => HasEffectActivated = false;
            }
        }

        public override void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            HasEffectActivated = false;

            Player.OnCardPlayedEvent -= AddActionOnPlantPlayedEvent;
            Player.OppositePlayer.OnCardPlayedEvent -= AddActionOnPlantPlayedEvent;
        }
    }
}