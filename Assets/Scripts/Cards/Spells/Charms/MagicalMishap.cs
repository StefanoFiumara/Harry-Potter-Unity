using UnityEngine;
using System.Collections;

public class MagicalMishap : GenericSpell {

    public override void OnPlayAction()
    {
        _Player._OppositePlayer.TakeDamage(3);
    }


}
