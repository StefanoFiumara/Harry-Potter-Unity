using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class VanishingStep : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.OnNextTurnStartEvent += () => Player.OppositePlayer.AddActions(-1);
        }
    }
}