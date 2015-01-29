using System;
using System.Collections.Generic;

namespace Assets.Scripts.Cards.Spells
{
    public class DirectDamageSpell : GenericSpell {

        public int DamageAmount;

        public override void OnPlayAction()
        {
            _Player._OppositePlayer.TakeDamage(DamageAmount);
        }

        public override bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }

        public override void AfterInputAction(List<GenericCard> selectedCards)
        {
            throw new Exception("AfterInputAction called on DirectDamageSpell, this should never happen!");
        }

        protected override List<GenericCard> GetValidCards()
        {
            throw new Exception("GetValidCards called on DirectDamageSpell, this should never happen!");
        }
    }
}
