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

            this.Player.OnCardPlayedEvent += this.AddActionOnPlantPlayedEvent;
            this.Player.OppositePlayer.OnCardPlayedEvent += this.AddActionOnPlantPlayedEvent;
        }

        private void AddActionOnPlantPlayedEvent(BaseCard card, List<BaseCard> targets)
        {
            if (this.HasEffectActivated == false && card.HasTag(Tag.Plant))
            {
                this.HasEffectActivated = true;

                var player = this.Player.CanUseActions() ? this.Player : this.Player.OppositePlayer;

                player.AddActions(1);

                player.OppositePlayer.OnNextTurnStartEvent += () => this.HasEffectActivated = false;
            }
        }

        public override void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            this.HasEffectActivated = false;

            this.Player.OnCardPlayedEvent -= this.AddActionOnPlantPlayedEvent;
            this.Player.OppositePlayer.OnCardPlayedEvent -= this.AddActionOnPlantPlayedEvent;
        }
    }
}