using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Diffindo : GenericSpell {

    //private List<GenericCard> SelectedCards;

    public override void OnPlayAction()
    {
        //Move ALL invalid colliders to ignoreraycast layer
        _Player.DisableAllCards();
        _Player._OppositePlayer.DisableAllCards();
        
        //place valid cards in valid layer
        foreach(var card in _Player._OppositePlayer._InPlay.Cards) {
            card.Enable();
            card.gameObject.layer = Helper.VALID_CHOICE_LAYER;
        }

        var SelectedCards = new List<GenericCard>();
        StartCoroutine(WaitForPlayerInput(SelectedCards));
    }

    public override bool MeetsAdditionalPlayRequirements()
    {
        return _Player._OppositePlayer._InPlay.Cards.Count > 0;
    }

    public override void AfterInputAction(List<GenericCard> selectedCards)
    {
        if (selectedCards.Count == 1)
        {
            selectedCards[0].Enable();

            _Player._OppositePlayer._InPlay.Remove(selectedCards[0]);
            _Player._OppositePlayer._Discard.Add(selectedCards[0], 0.1f);

            //reset the layer for all the cards
            _Player._Deck.gameObject.layer = 0;
            _Player._OppositePlayer._Deck.gameObject.layer = 0;

            Helper.EnableCards(_Player._Hand.Cards);
            Helper.EnableCards(_Player._InPlay.Cards);
            Helper.EnableCards(_Player._OppositePlayer._Hand.Cards);
            Helper.EnableCards(_Player._OppositePlayer._InPlay.Cards);
        }
        else
        {
            throw new Exception("More than one input sent to Diffindo, this should never happen!");
        }
    }
}
