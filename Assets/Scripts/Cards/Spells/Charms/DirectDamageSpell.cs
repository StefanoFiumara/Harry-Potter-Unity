using UnityEngine;
using System.Collections;

public class DirectDamageSpell : GenericSpell {

    public int damageAmount;

    public override void OnPlayAction()
    {
        _Player._OppositePlayer.TakeDamage(damageAmount);
    }

    public override bool MeetsPlayRequirements()
    {
        return true;
    }

}
