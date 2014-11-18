using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Diffindo : GenericSpell {

    public override bool MeetsAdditionalPlayRequirements()
    {
        return _Player._OppositePlayer._InPlay.Cards.Count > 0;
    }

    protected override List<GenericCard> GetValidCards()
    {
        return _Player._OppositePlayer._InPlay.Cards;
    }
    public override void AfterInputAction(List<GenericCard> selectedCards)
    {
        if (selectedCards.Count == 1)
        {
            selectedCards[0].Enable();

            _Player._OppositePlayer._InPlay.Remove(selectedCards[0]);
            _Player._OppositePlayer._Discard.Add(selectedCards[0]);
        }
        else
        {
            throw new Exception("More than one input sent to Diffindo, this should never happen!");
        }
    }

    public override void OnPlayAction()
    {
        throw new Exception("OnPlayAction called on Diffindo, this should never happen!");
    }
}
