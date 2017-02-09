using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class VanishingStep : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            this.Player.OppositePlayer.OnNextTurnStartEvent += () => this.Player.OppositePlayer.AddActions(-1);
        }
    }
}