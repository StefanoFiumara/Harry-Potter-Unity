using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiceToSnuffboxes : GenericSpell {

    protected override List<GenericCard> GetValidCards()
    {
        var validCards = _Player._InPlay.GetCreaturesInPlay();
        validCards.AddRange(_Player._OppositePlayer._InPlay.GetCreaturesInPlay());

        return validCards;
    }
    public override bool MeetsAdditionalPlayRequirements()
    {
        //There must be at least 2 creatures in play
        var validCards = _Player._InPlay.GetCreaturesInPlay();
        validCards.AddRange(_Player._OppositePlayer._InPlay.GetCreaturesInPlay());

        return validCards.Count >= 2;
    }

    public override void AfterInputAction(List<GenericCard> selectedCards)
    {
        foreach(var card in selectedCards) {
            card._Player._InPlay.Remove(card);
            card._Player._Hand.Add(card, false, false);
        }
    }

    public override void OnPlayAction()
    {
        throw new Exception("OnPlayAction called on MiceToSnuffboxes, this should never happen!");
    }
}
