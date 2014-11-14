using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiceToSnuffboxes : GenericSpell {

    //private List<GenericCard> SelectedCards;

    public override void OnPlayAction()
    {
        //Move ALL invalid colliders to ignoreraycast layer
        _Player.DisableAllCards();
        _Player._OppositePlayer.DisableAllCards();

        //Get the list of valid cards (all creatures)
        var validCards = _Player._InPlay.GetCreaturesInPlay();
        validCards.AddRange(_Player._OppositePlayer._InPlay.GetCreaturesInPlay());

        //Enable the valid cards and add the to the right layer
        foreach(var card in validCards)
        {
            card.Enable();
            card.gameObject.layer = Helper.VALID_CHOICE_LAYER;
        }

        var SelectedCards = new List<GenericCard>();
        StartCoroutine(WaitForPlayerInput(SelectedCards));
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
            Helper.RotateCard(card.transform);
        }

        _Player.EnableAllCards();
        _Player._OppositePlayer.EnableAllCards();
    }
}
