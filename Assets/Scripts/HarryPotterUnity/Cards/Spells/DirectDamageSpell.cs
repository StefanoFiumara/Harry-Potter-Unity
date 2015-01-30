using System.Collections.Generic;

namespace Assets.Scripts.HarryPotterUnity.Cards.Spells
{
    public class DirectDamageSpell : GenericSpell {

        public int DamageAmount;

        public override void OnPlayAction()
        {
            Player.OppositePlayer.TakeDamage(DamageAmount);
        }

        public override bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }

        public override void AfterInputAction(List<GenericCard> selectedCards) { }

        protected override List<GenericCard> GetValidCards()
        {
            return null;
        }
    }
}
