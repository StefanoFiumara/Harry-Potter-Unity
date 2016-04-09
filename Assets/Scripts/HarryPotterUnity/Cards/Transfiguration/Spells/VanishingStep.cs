using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class VanishingStep : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.OnNextTurnStart += () => Player.OppositePlayer.AddActions(-1);
        }
    }
}