using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Spells
{
    public class DirectDamageSpell : GenericSpell {

        public int DamageAmount;

        public override void OnPlayAction()
        {
            Player.OppositePlayer.TakeDamage(DamageAmount);
        }
    }
}
