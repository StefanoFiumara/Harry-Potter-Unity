﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class DirectDamageSpell : GenericSpell {

    public int damageAmount;

    public override void OnPlayAction()
    {
        _Player._OppositePlayer.TakeDamage(damageAmount);
    }

    public override bool MeetsAdditionalPlayRequirements()
    {
        return true;
    }

    public override void AfterInputAction(List<GenericCard> selectedCards)
    {
        throw new Exception("AfterInputAction called on DirectDamageSpell, this should never happen.");
    }

}